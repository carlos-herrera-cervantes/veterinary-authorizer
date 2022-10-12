using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services.Types;

public class UserCreatedEvent
{
    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("roles")]
    public List<string> Roles { get; set; }
}
