using Volkswagen.Dashboard.Web.Models;

namespace Volkswagen.Dashboard.Web.Services;

public interface ICarService
{
    Task<List<CarModel>> GetCarsAsync(string token);
    Task<CarModel?> GetCarByIdAsync(int id);
    Task<bool> CreateCarAsync(CarModel car);
    Task<bool> UpdateCarAsync(int id, CarModel car);
    Task<bool> DeleteCarAsync(int id);
}
