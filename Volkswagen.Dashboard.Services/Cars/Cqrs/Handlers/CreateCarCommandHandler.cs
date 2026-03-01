using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars.Cqrs.Commands;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Handlers;

public sealed class CreateCarCommandHandler : IRequestHandler<CreateCarCommand, string>
{
    private readonly ICarsRepository _carsRepository;

    public CreateCarCommandHandler(ICarsRepository carsRepository)
    {
        _carsRepository = carsRepository;
    }

    public async Task<string> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        var car = new CarModel
        {
            Name = request.Name,
            DateRelease = request.DateRelease
        };

        return await _carsRepository.InsertCar(car);
    }
}
