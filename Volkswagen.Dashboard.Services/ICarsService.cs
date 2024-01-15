using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services
{
    public interface ICarsService
    {
        int CreateCar(CarModel carModel);
        CarModel GetCarById(int id);
        Task<IEnumerable<CarModel>> GetCars();
    }
}
