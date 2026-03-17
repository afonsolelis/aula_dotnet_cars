using MediatR;
using Volkswagen.Dashboard.Services.CQRS.Commands;

namespace Volkswagen.Dashboard.WebApi.GraphQL;

public class CarMutation
{
    public async Task<string> InsertCar(
        string name,
        DateTime dateRelease,
        [Service] IMediator mediator)
        => await mediator.Send(new InsertCarCommand(name, dateRelease));

    public async Task<string> UpdateCar(
        string id,
        string name,
        DateTime dateRelease,
        [Service] IMediator mediator)
        => await mediator.Send(new UpdateCarCommand(id, name, dateRelease));

    public async Task<bool> DeleteCar(
        string id,
        [Service] IMediator mediator)
    {
        await mediator.Send(new DeleteCarCommand(id));
        return true;
    }
}
