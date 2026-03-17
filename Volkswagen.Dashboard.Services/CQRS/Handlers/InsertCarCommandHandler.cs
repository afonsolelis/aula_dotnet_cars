using MediatR;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Commands;

namespace Volkswagen.Dashboard.Services.CQRS.Handlers;

public class InsertCarCommandHandler : IRequestHandler<InsertCarCommand, string>
{
    private readonly ICarsService _carsService;

    public InsertCarCommandHandler(ICarsService carsService)
    {
        _carsService = carsService;
    }

    public Task<string> Handle(InsertCarCommand request, CancellationToken cancellationToken)
        => _carsService.CreateCarAsync(new CreateCarInput(request.Name, request.DateRelease));
}
