using Newtonsoft.Json;

namespace TwitchBot.src
{
  class AuthResponse
  {
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
  }
}
