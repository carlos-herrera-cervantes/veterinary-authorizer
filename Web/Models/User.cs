using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Web.Models
{
    public class CreateUser
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
