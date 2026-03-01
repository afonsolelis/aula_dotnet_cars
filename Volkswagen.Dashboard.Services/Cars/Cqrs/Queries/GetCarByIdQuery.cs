using MediatR;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Queries;

public sealed record GetCarByIdQuery(string Id) : IRequest<CarModel?>;
