using MediatR;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Queries;

public sealed record GetCarsQuery : IRequest<IEnumerable<CarModel>>;
