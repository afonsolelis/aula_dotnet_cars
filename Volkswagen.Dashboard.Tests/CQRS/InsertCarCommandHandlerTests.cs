using Moq;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Commands;
using Volkswagen.Dashboard.Services.CQRS.Handlers;

namespace Volkswagen.Dashboard.Tests.CQRS;

[TestFixture]
public class InsertCarCommandHandlerTests
{
    private Mock<ICarsService> _serviceMock = null!;
    private InsertCarCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _serviceMock = new Mock<ICarsService>();
        _handler = new InsertCarCommandHandler(_serviceMock.Object);
    }

    [Test]
    public async Task Handle_ShouldCallInsertCarAndReturnId()
    {
        // Arrange
        const string expectedId = "65f0d5934f4f35f8d2cd2001";
        _serviceMock
            .Setup(s => s.CreateCarAsync(It.IsAny<CreateCarInput>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _handler.Handle(
            new InsertCarCommand("Passat", DateTime.UtcNow),
            CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedId));
        _serviceMock.Verify(
            s => s.CreateCarAsync(It.Is<CreateCarInput>(c => c.Name == "Passat")),
            Times.Once);
    }
}
