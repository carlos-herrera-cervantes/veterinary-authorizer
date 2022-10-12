using Newtonsoft.Json;

namespace Web.Types;

public class HttpResponseMessage
{
    [JsonProperty("message")]
    public string Message { get; set; }
}
