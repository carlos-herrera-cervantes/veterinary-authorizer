using Newtonsoft.Json;

namespace Services.Types;

public class UserVerificationEvent
{
    [JsonProperty("to")]
    public string To { get; set; }

    [JsonProperty("subject")]
    public string Subject { get; set; }

    [JsonProperty("body")]
    public string Body { get; set; }

    [JsonIgnore]
    public string UserType { get; set; }

    [JsonIgnore]
    public string Jwt { get; set; }
}
