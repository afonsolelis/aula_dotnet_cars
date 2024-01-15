
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services
{
    public class CarService : ICarsService
    {
        private readonly ICarsRepository _carsRepository;

        public CarService(ICarsRepository carsRepository)
        {
            _carsRepository = carsRepository;
        }


        public int CreateCar(CarModel carModel)
        {
            
            return carModel.Id;
        }

        public CarModel GetCarById(int id) => null;

        public async Task<IEnumerable<CarModel>> GetCars() => await _carsRepository.getAll();
    }
}
