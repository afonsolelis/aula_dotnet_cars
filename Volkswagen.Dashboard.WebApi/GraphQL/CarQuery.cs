using MediatR;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.WebApi.GraphQL;

public class CarQuery
{
    public async Task<IReadOnlyCollection<CarDto>> GetCars(
        [Service] IMediator mediator)
        => await mediator.Send(new GetCarsQuery());

    public async Task<CarDto?> GetCarById(
        string id,
        [Service] IMediator mediator)
        => await mediator.Send(new GetCarByIdQuery(id));
}
