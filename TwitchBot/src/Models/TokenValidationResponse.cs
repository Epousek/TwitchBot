using Newtonsoft.Json;

namespace TwitchBot.src.Models
{
  class TokenValidationResponse
  {
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
  }
}
