using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.Cars
{
    public interface ICarsService
    {
        string CreateCar(CarModel carModel);
        Task<CarModel?> GetCarById(string id);
        Task<IEnumerable<CarModel>> GetCars();
        Task<string> InsertCar(CarModel carModel);
        Task DeleteCar(string id);
    }
}
