using Newtonsoft.Json;

namespace TwitchBot.Models
{
  internal class Credentials
  {
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    [JsonProperty("secret")]
    public string Secret { get; set; }

    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("connection_string")]
    public string ConnectionString { get; set; }

    [JsonProperty("deepl_key")] 
    public string DeeplKey { get; set; }
  }
}
