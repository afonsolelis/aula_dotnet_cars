namespace Volkswagen.Dashboard.Services.Shipping;

public interface ICepLookupService
{
    Task<CepAddress?> GetAddressByCepAsync(string cep, CancellationToken cancellationToken = default);
}
