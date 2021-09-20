using Newtonsoft.Json;

namespace TwitchBot.src.Models
{
  class Credentials
  {
    [JsonProperty("Username")]
    public string Username { get; set; }

    [JsonProperty("ClientID")]
    public string ClientID { get; set; }

    [JsonProperty("Secret")]
    public string Secret { get; set; }

    [JsonProperty("AccessToken")]
    public string AccessToken { get; set; }

    [JsonProperty("RefreshToken")]
    public string RefreshToken { get; set; }

    [JsonProperty("ConnectionString")]
    public string ConnectionString { get; set; }
  }
}
