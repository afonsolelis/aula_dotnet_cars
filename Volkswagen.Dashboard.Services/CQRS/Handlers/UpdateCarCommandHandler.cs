using MediatR;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Commands;

namespace Volkswagen.Dashboard.Services.CQRS.Handlers;

public class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, string>
{
    private readonly ICarsService _carsService;

    public UpdateCarCommandHandler(ICarsService carsService)
    {
        _carsService = carsService;
    }

    public Task<string> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
        => _carsService.InsertCar(request.CarModel);
}
