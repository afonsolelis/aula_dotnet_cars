using MediatR;
using Volkswagen.Dashboard.Services.Cars;

namespace Volkswagen.Dashboard.Services.CQRS.Queries;

public record GetCarByIdQuery(string Id) : IRequest<CarDto?>;
