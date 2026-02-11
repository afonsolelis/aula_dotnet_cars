using Moq;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars;

namespace Volkswagen.Dashboard.Tests
{
    public class Tests
    {
        private Mock<ICarsRepository> _mock = null!;
        private ICarsService _carsService = null!;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<ICarsRepository>();
            _carsService = new CarsService(_mock.Object);
        }

        [Test]
        public void Should_GetCarsWithSuccess()
        {
            var expectedResult = new List<CarModel>
            {
                new() { Id = "65f0d5934f4f35f8d2cd1001", Name = "Fox", DateRelease = new DateTime(2022, 1, 1) },
                new() { Id = "65f0d5934f4f35f8d2cd1002", Name = "Polo", DateRelease = new DateTime(2022, 1, 1) },
                new() { Id = "65f0d5934f4f35f8d2cd1003", Name = "Gol", DateRelease = new DateTime(2022, 1, 1) },
                new() { Id = "65f0d5934f4f35f8d2cd1004", Name = "Passat", DateRelease = new DateTime(2022, 1, 1) }
            };

            _mock.Setup(x => x.GetCars())
                .ReturnsAsync(expectedResult);

            var result = _carsService.GetCars()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .ToList();

            Assert.That(result.First().Id, Is.EqualTo(expectedResult.First().Id));
            Assert.That(result.First().Name, Is.EqualTo(expectedResult.First().Name));
            Assert.That(result.First().DateRelease, Is.EqualTo(expectedResult.First().DateRelease));
        }

        [Test]
        public void Should_GetCarByIdWithSuccess()
        {
            const string carId = "65f0d5934f4f35f8d2cd1001";
            var expectedResult = new CarModel { Id = carId, Name = "Fox", DateRelease = new DateTime(2022, 1, 1) };

            _mock.Setup(x => x.GetCarById(carId))
                .ReturnsAsync(expectedResult);

            var result = _carsService.GetCarById(carId)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(expectedResult.Id));
            Assert.That(result.Name, Is.EqualTo(expectedResult.Name));
            Assert.That(result.DateRelease, Is.EqualTo(expectedResult.DateRelease));
        }
    }
}
