namespace Volkswagen.Dashboard.Repository
{
    public interface ICarsRepository
    {
        Task<IEnumerable<CarModel>> GetCars();
        Task<CarModel?> GetCarById(string id);
        Task<string> InsertCar(CarModel carModel);
        Task DeleteCar(string id);
        Task<string> UpdateCar(CarModel carModel);
    }
}
