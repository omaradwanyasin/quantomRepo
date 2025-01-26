using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace backend_api.Models
{
    public class Area
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // MongoDB _id field

        [BsonElement("areaName")]
        public string AreaName { get; set; }

        [BsonElement("areaStatus")]
        public bool AreaStatus { get; set; }

        [BsonElement("id")]  // Renamed this property to avoid conflict
        [BsonRepresentation(BsonType.Int64)]
        public int AreaId { get; set; } // Use a distinct name to avoid conflict with _id
    }
}
