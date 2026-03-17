using Moq;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Handlers;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.Tests.CQRS;

[TestFixture]
public class GetCarByIdQueryHandlerTests
{
    private Mock<ICarsService> _serviceMock = null!;
    private GetCarByIdQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _serviceMock = new Mock<ICarsService>();
        _handler = new GetCarByIdQueryHandler(_serviceMock.Object);
    }

    [Test]
    public async Task Handle_WhenCarExists_ShouldReturnCar()
    {
        // Arrange
        const string id = "65f0d5934f4f35f8d2cd1001";
        var expected = new CarDto(id, "Gol", DateTime.UtcNow);
        _serviceMock.Setup(s => s.GetCarByIdAsync(id)).ReturnsAsync(expected);

        // Act
        var result = await _handler.Handle(new GetCarByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
        Assert.That(result.Name, Is.EqualTo("Gol"));
    }

    [Test]
    public async Task Handle_WhenCarDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetCarByIdAsync(It.IsAny<string>())).ReturnsAsync((CarDto?)null);

        // Act
        var result = await _handler.Handle(
            new GetCarByIdQuery("000000000000000000000000"), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}
