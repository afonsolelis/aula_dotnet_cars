using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.Cars
{
    public class CarsService : ICarsService
    {
        private readonly ICarsRepository _carsRepository;

        public CarsService(ICarsRepository carsRepository)
        {
            _carsRepository = carsRepository;
        }

        public string CreateCar(CarModel carModel)
        {
            return carModel.Id;
        }

        public async Task<CarModel?> GetCarById(string id) => await _carsRepository.GetCarById(id);

        public async Task<IEnumerable<CarModel>> GetCars() => await _carsRepository.GetCars();

        public async Task<string> InsertCar(CarModel carModel)
        {
            if (!string.IsNullOrWhiteSpace(carModel.Id))
            {
                return await _carsRepository.UpdateCar(carModel);
            }

            return await _carsRepository.InsertCar(carModel);
        }

        public async Task DeleteCar(string id) => await _carsRepository.DeleteCar(id);
    }
}
