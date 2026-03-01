using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars.Cqrs.Queries;
using Volkswagen.Dashboard.WebApi.Protos;

namespace Volkswagen.Dashboard.WebApi.Grpc;

public sealed class CarGrpcService : CarGrpc.CarGrpcBase
{
    private readonly IMediator _mediator;

    public CarGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetCarsReply> GetCars(Empty request, ServerCallContext context)
    {
        var cars = await _mediator.Send(new GetCarsQuery());

        var response = new GetCarsReply();
        response.Cars.AddRange(cars.Select(Map));
        return response;
    }

    public override async Task<GetCarByIdReply> GetCarById(GetCarByIdRequest request, ServerCallContext context)
    {
        var car = await _mediator.Send(new GetCarByIdQuery(request.Id));

        if (car is null)
        {
            return new GetCarByIdReply { Found = false };
        }

        return new GetCarByIdReply
        {
            Found = true,
            Car = Map(car)
        };
    }

    private static CarItem Map(CarModel car)
    {
        return new CarItem
        {
            Id = car.Id,
            Name = car.Name,
            DateRelease = Timestamp.FromDateTime(car.DateRelease.ToUniversalTime())
        };
    }
}
