using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.CQRS.Commands;

namespace Volkswagen.Dashboard.WebApi.GraphQL;

public class CarMutation
{
    public async Task<string> InsertCar(
        string name,
        DateTime dateRelease,
        [Service] IMediator mediator)
    {
        var model = new CarModel { Name = name, DateRelease = dateRelease };
        return await mediator.Send(new InsertCarCommand(model));
    }

    public async Task<string> UpdateCar(
        string id,
        string name,
        DateTime dateRelease,
        [Service] IMediator mediator)
    {
        var model = new CarModel { Id = id, Name = name, DateRelease = dateRelease };
        return await mediator.Send(new UpdateCarCommand(model));
    }

    public async Task<bool> DeleteCar(
        string id,
        [Service] IMediator mediator)
    {
        await mediator.Send(new DeleteCarCommand(id));
        return true;
    }
}
