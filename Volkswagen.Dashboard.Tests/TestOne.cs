using Moq;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars;

namespace Volkswagen.Dashboard.Tests
{
    public class Tests
    {
        private Mock<ICarsRepository> _mock;
        private ICarsService _carsService;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<ICarsRepository>();
            _carsService = new CarsService(_mock.Object);
        }

        [Test]
        public void Should_GetCarsWithSuccess()
        {
            #region Arrange
            var expectedResult = new List<CarModel>()
            {
                new() { Id = 1, Name = "Fox", DateRelease = new DateTime(2022, 1, 1) },
                new() { Id = 2, Name = "Polo", DateRelease = new DateTime(2022, 1, 1) },
                new() { Id = 3, Name = "Gol", DateRelease = new DateTime(2022, 1, 1) },
                new() { Id = 4, Name = "Passat", DateRelease = new DateTime(2022, 1, 1) }
            };

            _mock.Setup(x => x.GetCars())
                 .ReturnsAsync(expectedResult);

            #endregion

            #region Act
            var result = _carsService.GetCars()
                                     .ConfigureAwait(false)
                                     .GetAwaiter()
                                     .GetResult();
            #endregion

            #region Assert
            Assert.That(result.First().Id, Is.EqualTo(expectedResult.Last().Id));
            Assert.That(result.First().Name, Is.EqualTo(expectedResult.First().Name));
            Assert.That(result.First().DateRelease, Is.EqualTo(expectedResult.First().DateRelease));
            #endregion
        }

        [Test]
        public void Should_GetCarByIdWithSuccess()
        {
            #region Arrange
            var expectedResult = new CarModel() { Id = 1, Name = "Fox", DateRelease = new DateTime(2022, 1, 1) };

            //_mock.Setup(x => x.GetCarById(It.IsAny<int>()))
            //     .ReturnsAsync(expectedResult);

            _mock.Setup(x => x.GetCarById(1))
                 .ReturnsAsync(expectedResult);

            #endregion

            #region Act
            var result = _carsService.GetCarById(1)
                                     .ConfigureAwait(false)
                                     .GetAwaiter()
                                     .GetResult();
            #endregion

            #region Assert
            Assert.That(result.Id, Is.EqualTo(expectedResult.Id));
            Assert.That(result.Name, Is.EqualTo(expectedResult.Name));
            Assert.That(result.DateRelease, Is.EqualTo(expectedResult.DateRelease));
            #endregion
        }
    }
}