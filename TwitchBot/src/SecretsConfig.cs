using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using TwitchBot.src.Models;
using System.Threading.Tasks;

namespace TwitchBot.src
{
  static class SecretsConfig
  {
    [JsonProperty("Credentials")]
    public static Credentials Credentials { get; set; }
    static readonly string path = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");

    public static async Task SetConfig()
    {
      if (File.Exists(path))
        Credentials = JObject.Parse(File.ReadAllText(path)).ToObject<Credentials>();
      else
        throw new FileNotFoundException("NO CONFIG FILE!!");
    }

    public static async Task SetToken(AppAccessToken token)
    {
      Credentials.AccessToken = token.AccessToken;
      //Credentials.RefreshToken = tokens.RefreshToken;
      await UpdateFileAsync();
    }

    private static async Task UpdateFileAsync()
    {
      await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(Credentials));
    }
  }
}
