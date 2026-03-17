using HotChocolate.Types;
using Volkswagen.Dashboard.Services.Cars;

namespace Volkswagen.Dashboard.WebApi.GraphQL;

public class CarType : ObjectType<CarDto>
{
    protected override void Configure(IObjectTypeDescriptor<CarDto> descriptor)
    {
        descriptor.Name("Car");
        descriptor.Field(c => c.Id).Name("id");
        descriptor.Field(c => c.Name).Name("name");
        descriptor.Field(c => c.DateRelease).Name("dateRelease");
    }
}
