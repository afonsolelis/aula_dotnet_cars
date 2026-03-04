using Grpc.Core;
using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.CQRS.Commands;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.WebApi.Grpc;

// Namespace corresponde ao csharp_namespace definido em car.proto
public class CarGrpcService : CarService.CarServiceBase
{
    private readonly IMediator _mediator;

    public CarGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetCarsResponse> GetCars(
        GetCarsRequest request, ServerCallContext context)
    {
        var cars = await _mediator.Send(new GetCarsQuery(), context.CancellationToken);
        var response = new GetCarsResponse();
        response.Cars.AddRange(cars.Select(ToProto));
        return response;
    }

    public override async Task<CarResponse> GetCarById(
        GetCarByIdRequest request, ServerCallContext context)
    {
        var car = await _mediator.Send(new GetCarByIdQuery(request.Id), context.CancellationToken);
        if (car is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Carro não encontrado"));
        return ToProto(car);
    }

    public override async Task<InsertCarResponse> InsertCar(
        InsertCarRequest request, ServerCallContext context)
    {
        var model = new CarModel
        {
            Name        = request.Name,
            DateRelease = DateTime.Parse(request.DateRelease, null,
                              System.Globalization.DateTimeStyles.RoundtripKind)
        };
        var id = await _mediator.Send(new InsertCarCommand(model), context.CancellationToken);
        return new InsertCarResponse { Id = id };
    }

    public override async Task<UpdateCarResponse> UpdateCar(
        UpdateCarRequest request, ServerCallContext context)
    {
        var model = new CarModel
        {
            Id          = request.Id,
            Name        = request.Name,
            DateRelease = DateTime.Parse(request.DateRelease, null,
                              System.Globalization.DateTimeStyles.RoundtripKind)
        };
        var id = await _mediator.Send(new UpdateCarCommand(model), context.CancellationToken);
        if (string.IsNullOrEmpty(id))
            throw new RpcException(new Status(StatusCode.NotFound, "Carro não encontrado"));
        return new UpdateCarResponse { Id = id };
    }

    public override async Task<DeleteCarResponse> DeleteCar(
        DeleteCarRequest request, ServerCallContext context)
    {
        await _mediator.Send(new DeleteCarCommand(request.Id), context.CancellationToken);
        return new DeleteCarResponse { Success = true };
    }

    // ── Mapeamento CarModel → proto ───────────────────────────────────────────
    private static CarResponse ToProto(CarModel car) => new()
    {
        Id          = car.Id,
        Name        = car.Name,
        DateRelease = car.DateRelease.ToString("O") // ISO 8601 round-trip
    };
}
