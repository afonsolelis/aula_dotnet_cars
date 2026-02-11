using System.Net.Http.Headers;
using System.Net.Http.Json;
using Volkswagen.Dashboard.Web.Models;

namespace Volkswagen.Dashboard.Web.Services;

public class CarService : ICarService
{
    private readonly HttpClient _httpClient;

    public CarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CarModel>> GetCarsAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetFromJsonAsync<List<CarModel>>("api/car");
        return response ?? new List<CarModel>();
    }

    public async Task<CarModel?> GetCarByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<CarModel>($"api/car/{id}");
    }

    public async Task<bool> CreateCarAsync(CarModel car)
    {
        var response = await _httpClient.PostAsJsonAsync("api/car", car);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCarAsync(string id, CarModel car)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/car/{id}", car);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCarAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/car/{id}");
        return response.IsSuccessStatusCode;
    }
}
