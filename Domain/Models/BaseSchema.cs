using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models;

public class BaseSchema
{
    [BsonElement("_id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("created_at")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [BsonElement("updated_at")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
