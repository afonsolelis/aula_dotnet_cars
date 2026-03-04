using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Volkswagen.Dashboard.WebApi.Grpc;

namespace Volkswagen.Dashboard.Tests.Integration;

/// <summary>
/// Testes black-box de integração gRPC.
/// Usa WebApplicationFactory para subir a API in-process — sem porta de rede.
/// </summary>
[TestFixture]
public class CarGrpcServiceTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private GrpcChannel _channel = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new WebApplicationFactory<Program>();
        var httpClient = _factory.CreateDefaultClient();
        _channel = GrpcChannel.ForAddress(
            _factory.Server.BaseAddress,
            new GrpcChannelOptions { HttpClient = httpClient });
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _channel.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetCars_ShouldReturnList()
    {
        // Arrange
        var client = new CarService.CarServiceClient(_channel);

        // Act
        var response = await client.GetCarsAsync(new GetCarsRequest());

        // Assert — valida contrato de saída (black-box)
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Cars, Is.Not.Null);
    }
}
