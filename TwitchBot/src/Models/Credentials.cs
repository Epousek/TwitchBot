using Newtonsoft.Json;

namespace TwitchBot.src.Models
{
  class Credentials
  {
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("client_id")]
    public string ClientID { get; set; }

    [JsonProperty("secret")]
    public string Secret { get; set; }

    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("RefreshToken")]
    public string RefreshToken { get; set; }

    [JsonProperty("connection_string")]
    public string ConnectionString { get; set; }
  }
}
