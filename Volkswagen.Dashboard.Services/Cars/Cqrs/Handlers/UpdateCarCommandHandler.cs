using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars.Cqrs.Commands;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Handlers;

public sealed class UpdateCarCommandHandler : IRequestHandler<UpdateCarCommand, string>
{
    private readonly ICarsRepository _carsRepository;

    public UpdateCarCommandHandler(ICarsRepository carsRepository)
    {
        _carsRepository = carsRepository;
    }

    public async Task<string> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {
        var car = new CarModel
        {
            Id = request.Id,
            Name = request.Name,
            DateRelease = request.DateRelease
        };

        return await _carsRepository.UpdateCar(car);
    }
}
