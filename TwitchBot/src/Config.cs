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
    static readonly string path = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");

    public static void SetConfig()
    {
      if (File.Exists(path))
        Credentials = JObject.Parse(File.ReadAllText(path)).ToObject<Credentials>();
      else
        throw new FileNotFoundException("NO CONFIG FILE!!");
    }

    public static void SetTokens(AuthResponse tokens)
    {
      Credentials.AccessToken = tokens.AccessToken;
      Credentials.RefreshToken = tokens.RefreshToken;
      UpdateFile();
    }

    private static void UpdateFile()
    {
      File.WriteAllText(path, JsonConvert.SerializeObject(Credentials));
    }
  }
}
