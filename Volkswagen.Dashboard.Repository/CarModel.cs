using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Volkswagen.Dashboard.Repository.Documents
{
    public class CarDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("dateRelease")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DateRelease { get; set; }
    }
}
