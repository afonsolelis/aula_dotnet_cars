using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services;

namespace Volkswagen.Dashboard.WebApi.Controllers
{
    [Route("api/car")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarsService _carsService;

        public CarController(ICarsService carsService)
        {
            _carsService = carsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCar()
        {
            return Ok(await _carsService.GetCars());
        }

        [HttpGet("{id}")]
        public IActionResult GetCar([FromRoute] int id)
        {
            var car = _carsService.GetCarById(id);
            if(car is null)
                return NotFound("Carro não encontrado!");

            return Ok(car);
        }

        [HttpPost]
        public IActionResult CreateCar([FromBody] CarModel carModel)
        {
            var id = _carsService.CreateCar(carModel);

            return Created("api/car", id);
        }
    }
}
