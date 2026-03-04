using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.CQRS.Commands;
using Volkswagen.Dashboard.Services.CQRS.Queries;
using Volkswagen.Dashboard.WebApi.Validators;

namespace Volkswagen.Dashboard.WebApi.Controllers
{
    [Route("api/car")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCars()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            TokenValidator.GetPermissionFromToken(token);
            return Ok(await _mediator.Send(new GetCarsQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar([FromRoute] string id)
        {
            var car = await _mediator.Send(new GetCarByIdQuery(id));
            if (car is null)
            {
                return NotFound("Carro não encontrado!");
            }

            return Ok(car);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar([FromBody] CarModel carModel)
        {
            var id = await _mediator.Send(new InsertCarCommand(carModel));
            return CreatedAtAction(nameof(GetCar), new { id }, new { id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar([FromRoute] string id)
        {
            await _mediator.Send(new DeleteCarCommand(id));
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar([FromBody] CarModel carModel, [FromRoute] string id)
        {
            carModel.Id = id;
            var result = await _mediator.Send(new UpdateCarCommand(carModel));
            if (string.IsNullOrWhiteSpace(result))
            {
                return NotFound("Carro não encontrado!");
            }

            return Ok(new { id = result });
        }
    }
}
