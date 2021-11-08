using Newtonsoft.Json;

namespace TwitchBot.src.Models
{
  public class TokenValidationResponse
  {
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
  }
}
