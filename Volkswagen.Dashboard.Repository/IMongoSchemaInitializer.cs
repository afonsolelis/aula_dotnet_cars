namespace Volkswagen.Dashboard.Repository
{
    public interface IMongoSchemaInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
