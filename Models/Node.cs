using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace backend_api.Models
{
    public class Node
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; } = string.Empty;

        [BsonElement("id")]  
        public int NodeId { get; set; }

        [BsonElement("x")]
        public double X { get; set; }

        [BsonElement("y")]
        public double Y { get; set; }

        [BsonElement("ids")]
        public List<int> ConnectedIds { get; set; } = new();

        [BsonElement("areaId")]
        public int AreaId { get; set; }

        [BsonElement("nodeName")]
        public string? NodeName { get; set; } = string.Empty;
    }
}
