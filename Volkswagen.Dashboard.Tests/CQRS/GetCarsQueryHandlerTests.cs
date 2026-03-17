using Moq;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Handlers;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.Tests.CQRS;

[TestFixture]
public class GetCarsQueryHandlerTests
{
    private Mock<ICarsService> _serviceMock = null!;
    private GetCarsQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _serviceMock = new Mock<ICarsService>();
        _handler = new GetCarsQueryHandler(_serviceMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnAllCars()
    {
        // Arrange
        var expectedCars = new List<CarDto>
        {
            new("65f0d5934f4f35f8d2cd1001", "Fox", new DateTime(2022, 1, 1)),
            new("65f0d5934f4f35f8d2cd1002", "Polo", new DateTime(2023, 1, 1))
        };
        _serviceMock.Setup(s => s.GetCarsAsync()).ReturnsAsync(expectedCars);

        // Act
        var result = (await _handler.Handle(new GetCarsQuery(), CancellationToken.None)).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo("Fox"));
        _serviceMock.Verify(s => s.GetCarsAsync(), Times.Once);
    }
}
