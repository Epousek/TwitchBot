using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using TwitchBot.src.Models;

namespace TwitchBot.src
{
  static class Config
  {
    [JsonProperty("Credentials")]
    public static Credentials Credentials { get; set; }

    public static void SetConfig()
    {
      string path = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");
      Credentials = JObject.Parse(File.ReadAllText(path)).ToObject<Credentials>();
    }

    public static void SetRefreshToken(string token)
    {
      Credentials.RefreshToken = token;
    }
  }
}
