using MediatR;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.CQRS.Queries;

public record GetCarsQuery() : IRequest<IEnumerable<CarModel>>;
