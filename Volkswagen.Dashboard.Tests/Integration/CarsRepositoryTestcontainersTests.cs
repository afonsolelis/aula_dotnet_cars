using MongoDB.Driver;
using Testcontainers.MongoDb;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Tests.Integration;

[TestFixture]
[Category("Testcontainers")]
public class CarsRepositoryTestcontainersTests
{
    private MongoDbContainer? _mongoContainer;
    private IMongoDatabase? _database;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        try
        {
            _mongoContainer = new MongoDbBuilder()
                .WithImage("mongo:7.0")
                .Build();

            await _mongoContainer.StartAsync();
        }
        catch (Exception ex)
        {
            Assert.Ignore($"Docker indisponivel para Testcontainers: {ex.Message}");
        }

        var client = new MongoClient(_mongoContainer.GetConnectionString());
        _database = client.GetDatabase($"volksdb_test_{Guid.NewGuid():N}");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_mongoContainer is not null)
        {
            await _mongoContainer.DisposeAsync();
        }
    }

    [Test]
    public async Task InsertAndGetById_ShouldPersistUsingRealMongoContainer()
    {
        Assert.That(_database, Is.Not.Null);

        var repository = new CarsRepository(_database!);
        var expectedDate = new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Utc);

        var id = await repository.InsertCar(new CarModel
        {
            Name = "Taos",
            DateRelease = expectedDate
        });

        var saved = await repository.GetCarById(id);

        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Name, Is.EqualTo("Taos"));
        Assert.That(saved.DateRelease, Is.EqualTo(expectedDate));
    }
}
