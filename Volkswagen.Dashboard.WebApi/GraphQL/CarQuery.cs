using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.WebApi.GraphQL;

public class CarQuery
{
    public async Task<IEnumerable<CarModel>> GetCars(
        [Service] IMediator mediator)
        => await mediator.Send(new GetCarsQuery());

    public async Task<CarModel?> GetCarById(
        string id,
        [Service] IMediator mediator)
        => await mediator.Send(new GetCarByIdQuery(id));
}
