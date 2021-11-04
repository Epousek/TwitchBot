using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using TwitchBot.src.Models;
using System.Threading.Tasks;
using Serilog;

namespace TwitchBot.src
{
  static class SecretsConfig
  {
    [JsonProperty("Credentials")]
    public static Credentials Credentials { get; set; }
    private static readonly string path = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");

    public static async Task SetConfig()
    {
      if (File.Exists(path))
      {
        Credentials = JObject.Parse(await File.ReadAllTextAsync(path).ConfigureAwait(false)).ToObject<Credentials>();
        Log.Information("Successfully obtained credentials from config.");
      }
      else
      {
        Log.Fatal("Config file not found in {path}!", path);
      }
    }

    public static async Task SetToken(AppAccessToken tokens)
    {
      Credentials.AccessToken = tokens.AccessToken;
      Credentials.RefreshToken = tokens.RefreshToken;
      Log.Information("Tokens set.");
      await UpdateFileAsync().ConfigureAwait(false);
    }

    private static async Task UpdateFileAsync()
    {
      if (File.Exists(path))
      {
        await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(Credentials)).ConfigureAwait(false);
        Log.Information("Config file updated.");
      }
      else
      {
        Log.Fatal("Config file not found in {path}!", path);
      }
    }
  }
}
