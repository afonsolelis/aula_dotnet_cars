using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Volkswagen.Dashboard.Tests.Support.Vcr;

public sealed class WeeklyVcrHandler : DelegatingHandler
{
    private readonly string _cassettePath;
    private readonly Func<DateTimeOffset> _utcNow;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public WeeklyVcrHandler(string cassettePath, Func<DateTimeOffset>? utcNow = null)
    {
        _cassettePath = cassettePath;
        _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method != HttpMethod.Get)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var now = _utcNow();
        var cassette = await ReadCassetteAsync(cancellationToken);

        if (cassette is not null && IsSameIsoWeek(cassette.RecordedAtUtc, now))
        {
            return CreatePlaybackResponse(cassette, request);
        }

        var response = await base.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        var toRecord = new CassetteRecord
        {
            Method = request.Method.Method,
            Url = NormalizeUrl(request.RequestUri?.ToString()),
            RecordedAtUtc = now,
            StatusCode = (int)response.StatusCode,
            ReasonPhrase = response.ReasonPhrase,
            MediaType = response.Content.Headers.ContentType?.MediaType,
            Content = payload
        };

        await WriteCassetteAsync(toRecord, cancellationToken);

        return response;
    }

    private async Task<CassetteRecord?> ReadCassetteAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_cassettePath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(_cassettePath, cancellationToken);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        return new CassetteRecord
        {
            Method = GetString(root, "method"),
            Url = GetString(root, "url"),
            RecordedAtUtc = GetDateTimeOffset(root, "recordedAtUtc"),
            StatusCode = GetInt(root, "statusCode"),
            ReasonPhrase = GetNullableString(root, "reasonPhrase"),
            MediaType = GetNullableString(root, "mediaType"),
            Content = GetString(root, "content")
        };
    }

    private async Task WriteCassetteAsync(CassetteRecord cassette, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_cassettePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_cassettePath);
        await JsonSerializer.SerializeAsync(stream, cassette, JsonOptions, cancellationToken);
    }

    private static HttpResponseMessage CreatePlaybackResponse(CassetteRecord cassette, HttpRequestMessage request)
    {
        var mediaType = string.IsNullOrWhiteSpace(cassette.MediaType) ? "application/json" : cassette.MediaType;

        return new HttpResponseMessage((HttpStatusCode)cassette.StatusCode)
        {
            RequestMessage = request,
            ReasonPhrase = cassette.ReasonPhrase,
            Content = new StringContent(cassette.Content, Encoding.UTF8, mediaType)
        };
    }

    private static bool IsSameIsoWeek(DateTimeOffset left, DateTimeOffset right)
    {
        return ISOWeek.GetYear(left.UtcDateTime) == ISOWeek.GetYear(right.UtcDateTime)
            && ISOWeek.GetWeekOfYear(left.UtcDateTime) == ISOWeek.GetWeekOfYear(right.UtcDateTime);
    }

    private static string NormalizeUrl(string? url)
    {
        return (url ?? string.Empty).TrimEnd('/');
    }

    private static string GetString(JsonElement root, string propertyName)
    {
        return GetPropertyIgnoreCase(root, propertyName).GetString() ?? string.Empty;
    }

    private static string? GetNullableString(JsonElement root, string propertyName)
    {
        return GetPropertyIgnoreCase(root, propertyName).GetString();
    }

    private static int GetInt(JsonElement root, string propertyName)
    {
        return GetPropertyIgnoreCase(root, propertyName).GetInt32();
    }

    private static DateTimeOffset GetDateTimeOffset(JsonElement root, string propertyName)
    {
        return GetPropertyIgnoreCase(root, propertyName).GetDateTimeOffset();
    }

    private static JsonElement GetPropertyIgnoreCase(JsonElement root, string propertyName)
    {
        foreach (var property in root.EnumerateObject())
        {
            if (property.NameEquals(propertyName) ||
                property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return property.Value;
            }
        }

        throw new KeyNotFoundException($"Propriedade '{propertyName}' nao encontrada no cassette.");
    }

    private sealed class CassetteRecord
    {
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTimeOffset RecordedAtUtc { get; set; }
        public int StatusCode { get; set; }
        public string? ReasonPhrase { get; set; }
        public string? MediaType { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
