using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Volkswagen.Dashboard.Services.Shipping;

public sealed class ViaCepLookupService : ICepLookupService
{
    private readonly HttpClient _httpClient;

    public ViaCepLookupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CepAddress?> GetAddressByCepAsync(string cep, CancellationToken cancellationToken = default)
    {
        var normalizedCep = NormalizeCep(cep);
        var response = await _httpClient.GetFromJsonAsync<ViaCepResponse>($"ws/{normalizedCep}/json/", cancellationToken);

        if (response is null || response.Erro)
        {
            return null;
        }

        return new CepAddress
        {
            Cep = response.Cep,
            Street = response.Logradouro,
            Neighborhood = response.Bairro,
            City = response.Localidade,
            State = response.Uf
        };
    }

    private static string NormalizeCep(string cep)
    {
        var digits = new string(cep.Where(char.IsDigit).ToArray());

        if (digits.Length != 8)
        {
            throw new ArgumentException("CEP deve conter 8 digitos.", nameof(cep));
        }

        return digits;
    }

    private sealed class ViaCepResponse
    {
        [JsonPropertyName("cep")]
        public string Cep { get; set; } = string.Empty;

        [JsonPropertyName("logradouro")]
        public string Logradouro { get; set; } = string.Empty;

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; } = string.Empty;

        [JsonPropertyName("localidade")]
        public string Localidade { get; set; } = string.Empty;

        [JsonPropertyName("uf")]
        public string Uf { get; set; } = string.Empty;

        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}
