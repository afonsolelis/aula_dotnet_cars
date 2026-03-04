using HotChocolate.Types;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.WebApi.GraphQL;

public class CarType : ObjectType<CarModel>
{
    protected override void Configure(IObjectTypeDescriptor<CarModel> descriptor)
    {
        descriptor.Name("Car");
        descriptor.Field(c => c.Id).Name("id");
        descriptor.Field(c => c.Name).Name("name");
        descriptor.Field(c => c.DateRelease).Name("dateRelease");
    }
}
