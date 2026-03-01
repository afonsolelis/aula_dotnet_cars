using MediatR;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Commands;

public sealed record DeleteCarCommand(string Id) : IRequest;
