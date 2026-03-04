using Moq;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Commands;
using Volkswagen.Dashboard.Services.CQRS.Handlers;

namespace Volkswagen.Dashboard.Tests.CQRS;

[TestFixture]
public class DeleteCarCommandHandlerTests
{
    private Mock<ICarsService> _serviceMock = null!;
    private DeleteCarCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _serviceMock = new Mock<ICarsService>();
        _handler = new DeleteCarCommandHandler(_serviceMock.Object);
    }

    [Test]
    public async Task Handle_ShouldCallDeleteCarOnce()
    {
        // Arrange
        const string id = "65f0d5934f4f35f8d2cd1001";
        _serviceMock.Setup(s => s.DeleteCar(id)).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(new DeleteCarCommand(id), CancellationToken.None);

        // Assert
        _serviceMock.Verify(s => s.DeleteCar(id), Times.Once);
    }
}
