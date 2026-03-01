using MongoDB.Driver;
using Testcontainers.MongoDb;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Tests.Integration;

[TestFixture]
[NonParallelizable]
public sealed class CarsRepositoryMongoContainerTests
{
    private MongoDbContainer _mongoContainer = null!;
    private string _connectionString = string.Empty;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        try
        {
            _mongoContainer = new MongoDbBuilder()
                .WithImage("mongo:7.0")
                .Build();

            await _mongoContainer.StartAsync();
            _connectionString = _mongoContainer.GetConnectionString();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Docker indisponivel para Testcontainers: {ex.Message}");
        }
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
    public async Task Should_Insert_And_Get_Car_Using_Mongo_Testcontainer()
    {
        var repository = CreateRepository();
        var car = new CarModel
        {
            Name = "Tiguan",
            DateRelease = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc)
        };

        var id = await repository.InsertCar(car);
        var loaded = await repository.GetCarById(id);

        Assert.That(loaded, Is.Not.Null);
        Assert.That(loaded!.Id, Is.EqualTo(id));
        Assert.That(loaded.Name, Is.EqualTo("Tiguan"));
    }

    [Test]
    public async Task Should_Update_Car_Using_Mongo_Testcontainer()
    {
        var repository = CreateRepository();

        var id = await repository.InsertCar(new CarModel
        {
            Name = "Nivus",
            DateRelease = new DateTime(2023, 8, 20, 0, 0, 0, DateTimeKind.Utc)
        });

        var updatedId = await repository.UpdateCar(new CarModel
        {
            Id = id,
            Name = "Nivus Highline",
            DateRelease = new DateTime(2023, 8, 20, 0, 0, 0, DateTimeKind.Utc)
        });

        var loaded = await repository.GetCarById(id);

        Assert.That(updatedId, Is.EqualTo(id));
        Assert.That(loaded, Is.Not.Null);
        Assert.That(loaded!.Name, Is.EqualTo("Nivus Highline"));
    }

    [Test]
    public void Should_Open_CircuitBreaker_When_Mongo_Is_Unavailable_Chaos()
    {
        var repository = CreateBrokenRepository();

        var ex = Assert.ThrowsAsync<RepositoryUnavailableException>(async () => await repository.GetCars());
        Assert.That(ex!.Message, Does.Contain("MongoDB indisponivel"));

        var fastFailure = Assert.ThrowsAsync<RepositoryUnavailableException>(async () => await repository.GetCars());
        Assert.That(fastFailure!.Message, Does.Contain("Circuit breaker aberto"));
    }

    [Test]
    public async Task Should_Recover_On_Healthy_Mongo_After_Chaos()
    {
        var repository = CreateRepository();

        var id = await repository.InsertCar(new CarModel
        {
            Name = "Amarok V6",
            DateRelease = new DateTime(2024, 10, 10, 0, 0, 0, DateTimeKind.Utc)
        });

        var loaded = await repository.GetCarById(id);

        Assert.That(loaded, Is.Not.Null);
        Assert.That(loaded!.Name, Is.EqualTo("Amarok V6"));
    }

    private CarsRepository CreateRepository()
    {
        var settings = MongoClientSettings.FromConnectionString(_connectionString);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(2);
        settings.ConnectTimeout = TimeSpan.FromSeconds(2);
        settings.SocketTimeout = TimeSpan.FromSeconds(2);

        var client = new MongoClient(settings);
        var database = client.GetDatabase($"volks-test-{Guid.NewGuid():N}");
        return new CarsRepository(database);
    }

    private CarsRepository CreateBrokenRepository()
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb://127.0.0.1:1");
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(1);
        settings.ConnectTimeout = TimeSpan.FromSeconds(1);
        settings.SocketTimeout = TimeSpan.FromSeconds(1);

        var client = new MongoClient(settings);
        var database = client.GetDatabase($"volks-broken-{Guid.NewGuid():N}");
        return new CarsRepository(database);
    }
}
