using Moq;
using Volkswagen.Dashboard.Domain.Cars;
using Volkswagen.Dashboard.Domain.Repositories;
using Volkswagen.Dashboard.Services.Cars;

namespace Volkswagen.Dashboard.Tests.Cars;

[TestFixture]
public class CarsServiceTests
{
    private Mock<ICarRepository> _repositoryMock = null!;
    private Mock<ICarDtoMapper> _mapperMock = null!;
    private CarsService _service = null!;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<ICarRepository>();
        _mapperMock = new Mock<ICarDtoMapper>();
        _service = new CarsService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GetCarsAsync_ShouldDelegateProjectionToMapper()
    {
        var gol = Car.Restore("1", "Gol", new DateTime(2020, 1, 1));
        var polo = Car.Restore("2", "Polo", new DateTime(2021, 1, 1));
        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new[] { gol, polo });
        _mapperMock.Setup(x => x.Map(gol)).Returns(new CarDto(gol.Id, gol.Name, gol.DateRelease));
        _mapperMock.Setup(x => x.Map(polo)).Returns(new CarDto(polo.Id, polo.Name, polo.DateRelease));

        var result = await _service.GetCarsAsync();

        Assert.That(result, Has.Count.EqualTo(2));
        _mapperMock.Verify(x => x.Map(gol), Times.Once);
        _mapperMock.Verify(x => x.Map(polo), Times.Once);
    }
}
