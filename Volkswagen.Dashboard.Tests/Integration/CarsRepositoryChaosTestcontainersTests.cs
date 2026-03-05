using System.Diagnostics;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Tests.Integration;

[TestFixture]
[Category("Testcontainers")]
[Category("Chaos")]
public class CarsRepositoryChaosTestcontainersTests
{
    private MongoDbContainer? _mongoContainer;
    private CarsRepository? _repository;

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
            Assert.Ignore($"Docker indisponivel para teste de caos com Testcontainers: {ex.Message}");
        }

        var connectionString = WithShortMongoTimeouts(_mongoContainer!.GetConnectionString());
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase($"volksdb_chaos_{Guid.NewGuid():N}");
        _repository = new CarsRepository(database);
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
    public async Task PauseAndResumeContainer_ShouldFailAndRecoverRepositoryAccess()
    {
        Assert.That(_mongoContainer, Is.Not.Null);
        Assert.That(_repository, Is.Not.Null);

        await _repository!.InsertCar(new CarModel
        {
            Name = "Nivus",
            DateRelease = DateTime.SpecifyKind(new DateTime(2026, 3, 4), DateTimeKind.Utc)
        });

        TryDockerCommand($"pause {_mongoContainer!.Id}", "Nao foi possivel pausar o container do teste de caos.");

        Assert.That(
            async () => await _repository.GetCars(),
            Throws.InstanceOf<MongoException>(),
            "A consulta deveria falhar enquanto o MongoDB esta pausado.");

        TryDockerCommand($"unpause {_mongoContainer.Id}", "Nao foi possivel retomar o container do teste de caos.");
        await Task.Delay(2000);

        var recovered = false;
        for (var attempt = 1; attempt <= 30; attempt++)
        {
            try
            {
                _ = await _repository.GetCars();
                recovered = true;
                break;
            }
            catch (Exception ex) when (ex is MongoException or TimeoutException)
            {
                await Task.Delay(2000);
            }
        }

        Assert.That(recovered, Is.True, "Repositorio nao recuperou apos o experimento de caos.");
    }

    private static string WithShortMongoTimeouts(string connectionString)
    {
        var separator = connectionString.Contains('?', StringComparison.Ordinal) ? "&" : "?";
        return $"{connectionString}{separator}serverSelectionTimeoutMS=2000&connectTimeoutMS=2000&socketTimeoutMS=2000";
    }

    private static void TryDockerCommand(string args, string ignoreMessage)
    {
        var startInfo = new ProcessStartInfo("docker", args)
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            Assert.Ignore(ignoreMessage);
            return;
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var stderr = process.StandardError.ReadToEnd();
            Assert.Ignore($"{ignoreMessage} Erro: {stderr}");
        }
    }
}
