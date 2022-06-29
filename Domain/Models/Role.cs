using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models
{
    public class Role : BaseSchema
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("active")]
        public bool Active { get; set; } = true;
    }
}
