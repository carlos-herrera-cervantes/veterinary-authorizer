using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models
{
    public class User : BaseSchema
    {
        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("roles")]
        public List<string> Roles { get; set; } = new List<string> { UserRole.Customer };

        [BsonElement("verified")]
        public bool Verified { get; set; } = false;

        [BsonElement("verification_token")]
        public string VerificationToken { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = UserType.Customer;

        [BsonElement("firebase_token")]
        public string FirebaseToken { get; set; }

        [BsonElement("block")]
        public bool Block { get; set; } = false;

        [BsonElement("deactivated")]
        public bool Deactivated { get; set; } = false;
    }
}
