using Moq;
using Volkswagen.Dashboard.Repository;
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
        _serviceMock.Setup(s => s.InsertCar(It.IsAny<CarModel>())).ReturnsAsync(expectedId);

        var model = new CarModel { Name = "Passat", DateRelease = DateTime.UtcNow };

        // Act
        var result = await _handler.Handle(new InsertCarCommand(model), CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedId));
        // Handler deve limpar o Id para forçar insert (não update)
        _serviceMock.Verify(s => s.InsertCar(It.Is<CarModel>(c => c.Id == string.Empty)), Times.Once);
    }
}
