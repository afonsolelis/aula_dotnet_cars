using System.Net;
using System.Text.Json;
using Volkswagen.Dashboard.Services.Shipping;
using Volkswagen.Dashboard.Tests.Support.Vcr;

namespace Volkswagen.Dashboard.Tests.Integration;

[TestFixture]
public sealed class ViaCepVcrTests
{
    private const string Cep = "01001000";

    [Test]
    public async Task Should_Call_Real_ViaCep_And_Refresh_Cassette_When_Week_Is_Stale()
    {
        var cassettePath = GetCassettePath();

        await File.WriteAllTextAsync(cassettePath, JsonSerializer.Serialize(new
        {
            method = "GET",
            url = $"https://viacep.com.br/ws/{Cep}/json/",
            recordedAtUtc = DateTimeOffset.UtcNow.AddDays(-10),
            statusCode = 200,
            reasonPhrase = "OK",
            mediaType = "application/json",
            content = "{\"cep\":\"01001-000\",\"logradouro\":\"Praca da Se\",\"bairro\":\"Se\",\"localidade\":\"Sao Paulo\",\"uf\":\"SP\"}"
        }));

        var countingHandler = new CountingHandler(new HttpClientHandler());
        var vcrHandler = new WeeklyVcrHandler(cassettePath)
        {
            InnerHandler = countingHandler
        };

        using var httpClient = new HttpClient(vcrHandler)
        {
            BaseAddress = new Uri("https://viacep.com.br/")
        };

        var service = new ViaCepLookupService(httpClient);

        var result = await service.GetAddressByCepAsync(Cep);
        var cassetteJson = await File.ReadAllTextAsync(cassettePath);
        using var doc = JsonDocument.Parse(cassetteJson);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.City, Is.Not.Empty);
        Assert.That(countingHandler.CallCount, Is.EqualTo(1));
        Assert.That(doc.RootElement.GetProperty("recordedAtUtc").GetDateTimeOffset(),
            Is.GreaterThan(DateTimeOffset.UtcNow.AddMinutes(-5)));
    }

    [Test]
    public async Task Should_Use_Weekly_Cassette_Without_Calling_Real_Api()
    {
        var cassettePath = GetCassettePath();

        await File.WriteAllTextAsync(cassettePath, JsonSerializer.Serialize(new
        {
            method = "GET",
            url = $"https://viacep.com.br/ws/{Cep}/json/",
            recordedAtUtc = DateTimeOffset.UtcNow,
            statusCode = 200,
            reasonPhrase = "OK",
            mediaType = "application/json",
            content = "{\"cep\":\"01001-000\",\"logradouro\":\"Praca da Se\",\"bairro\":\"Se\",\"localidade\":\"Sao Paulo\",\"uf\":\"SP\"}"
        }));

        var vcrHandler = new WeeklyVcrHandler(cassettePath)
        {
            InnerHandler = new ThrowIfCalledHandler()
        };

        using var httpClient = new HttpClient(vcrHandler)
        {
            BaseAddress = new Uri("https://viacep.com.br/")
        };

        var service = new ViaCepLookupService(httpClient);
        var result = await service.GetAddressByCepAsync(Cep);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.City, Is.EqualTo("Sao Paulo"));
    }

    private static string GetCassettePath()
    {
        var root = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", ".."));
        var cassettesDir = Path.Combine(root, "Volkswagen.Dashboard.Tests", "Cassettes");
        Directory.CreateDirectory(cassettesDir);
        return Path.Combine(cassettesDir, "viacep-01001000.json");
    }

    private sealed class CountingHandler : DelegatingHandler
    {
        public int CallCount { get; private set; }

        public CountingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return base.SendAsync(request, cancellationToken);
        }
    }

    private sealed class ThrowIfCalledHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new AssertionException("A chamada real nao deveria acontecer quando o VCR da semana existe.");
        }
    }
}
