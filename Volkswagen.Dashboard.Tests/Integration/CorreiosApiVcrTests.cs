using System.Net;
using System.Text;
using System.Text.Json;

namespace Volkswagen.Dashboard.Tests.Integration;

[TestFixture]
[Category("VCR")]
public class CorreiosApiVcrTests
{
    private const string Endpoint = "https://brasilapi.com.br/api/cep/v2/01001000";
    private const string CassetteRelativePath = "Integration/Cassettes/correios_cep_01001000.json";

    [Test]
    public async Task BuscarCep_ComVcr_DeveRetornarDadosDeterministicos()
    {
        var cassettePath = Path.Combine(TestContext.CurrentContext.TestDirectory, CassetteRelativePath);
        var mode = (Environment.GetEnvironmentVariable("VCR_MODE") ?? "replay").Trim().ToLowerInvariant();

        string responseBody;
        HttpStatusCode statusCode;

        if (mode == "record")
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(Endpoint);
            responseBody = await response.Content.ReadAsStringAsync();
            statusCode = response.StatusCode;

            var cassette = new VcrCassette
            {
                Method = "GET",
                Url = Endpoint,
                RecordedAtUtc = DateTime.UtcNow,
                StatusCode = (int)statusCode,
                ResponseBody = responseBody
            };

            Directory.CreateDirectory(Path.GetDirectoryName(cassettePath)!);
            await File.WriteAllTextAsync(cassettePath, JsonSerializer.Serialize(cassette, JsonOptions()), Encoding.UTF8);
        }
        else
        {
            Assert.That(File.Exists(cassettePath), Is.True, $"Cassette nao encontrada em: {cassettePath}");
            var cassetteJson = await File.ReadAllTextAsync(cassettePath, Encoding.UTF8);
            var cassette = JsonSerializer.Deserialize<VcrCassette>(cassetteJson, JsonOptions());

            Assert.That(cassette, Is.Not.Null);
            responseBody = cassette!.ResponseBody;
            statusCode = (HttpStatusCode)cassette.StatusCode;
        }

        Assert.That((int)statusCode, Is.EqualTo(200));

        using var payload = JsonDocument.Parse(responseBody);
        var root = payload.RootElement;

        Assert.That(root.TryGetProperty("cep", out _), Is.True);
        Assert.That(root.TryGetProperty("city", out _), Is.True);
        Assert.That(root.TryGetProperty("service", out _), Is.True);
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private sealed class VcrCassette
    {
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime RecordedAtUtc { get; set; }
        public int StatusCode { get; set; }
        public string ResponseBody { get; set; } = string.Empty;
    }
}
