using MediatR;
using Volkswagen.Dashboard.Services.Cars;

namespace Volkswagen.Dashboard.Services.CQRS.Queries;

public record GetCarsQuery() : IRequest<IReadOnlyCollection<CarDto>>;
