using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.WebApi.Validators;

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
        [Authorize]
        public async Task<IActionResult> GetCars()
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            TokenValidator.GetPermissionFromToken(token);
            return Ok(await _carsService.GetCars());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar([FromRoute] int id)
        {
            var car = await _carsService.GetCarById(id);
            if(car is null)
                return NotFound("Carro não encontrado!");
                
            return Ok(car);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar([FromBody] CarModel carModel)
        {
            var id = await _carsService.InsertCar(carModel);

            return Created("api/car", id);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCar([FromBody] int id)
        {
            await _carsService.DeleteCar(id);

            return Created("api/car", id);
        }

        /// <summary>
        /// descrição do método
        /// </summary>
        /// <param name="carModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar([FromBody] CarModel carModel, [FromRoute] int id)
        {
            carModel.Id = id;
            var result = await _carsService.InsertCar(carModel);

            return Created("api/car", result);
        }
    }
}
