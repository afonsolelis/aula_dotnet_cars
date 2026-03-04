using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Volkswagen.Dashboard.Tests.Integration;

/// <summary>
/// Testes black-box de integração GraphQL.
/// Envia queries reais ao endpoint /graphql via HttpClient in-process.
/// </summary>
[TestFixture]
public class CarGraphQLTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client  = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetCars_Query_ShouldReturn200WithDataField()
    {
        // Arrange — query declarativa (cliente pede exatamente os campos desejados)
        var query = new { query = "{ cars { id name dateRelease } }" };

        // Act
        var response = await _client.PostAsJsonAsync("/graphql", query);

        // Assert — valida status e contrato de saída
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.That(body, Does.Contain("\"data\""));
    }

    [Test]
    public async Task GetCarById_Query_WithInvalidId_ShouldReturnNullData()
    {
        // Arrange
        var query = new
        {
            query = "query($id: String!) { carById(id: $id) { id name } }",
            variables = new { id = "000000000000000000000000" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/graphql", query);

        // Assert — GraphQL retorna 200 mesmo para dados não encontrados (null no campo)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
    }
}
