using Moq;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services;

namespace Volkswagen.Dashboard.UnitTests
{
    public class CarServiceTests
    {
        private ICarsService _carsService;
        private Mock<ICarsRepository> _mock;

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
            var expectedResult = new List<CarModel> 
            { 
                new() { Id = 1, Name = "Fox", DateRelease = 2022 }, 
                new() { Id = 1, Name = "Fox", DateRelease = 2022 },
                new() { Id = 1, Name = "Fox", DateRelease = 2022 },
                new() { Id = 1, Name = "Fox", DateRelease = 2022 } 
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
            Assert.That(result.First().Id, Is.EqualTo(expectedResult[1].Id));
            Assert.That(result.First().Name, Is.EqualTo(expectedResult[1].Name));
            Assert.That(result.First().DateRelease, Is.EqualTo(expectedResult[1].DateRelease));
            #endregion
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}