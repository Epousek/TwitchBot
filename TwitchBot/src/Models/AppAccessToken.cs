using System;
using Newtonsoft.Json;

namespace TwitchBot.src.Models
{
  class AppAccessToken
  {
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
  }
}
