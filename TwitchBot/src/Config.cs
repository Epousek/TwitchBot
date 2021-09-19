using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using TwitchBot.src.Models;

namespace TwitchBot.src
{
  class Config
  {
    [JsonProperty("Credentials")]
    public Credentials Credentials { get; set; }

    public Config()
    {
      SetConfig();
    }

    public void SetConfig()
    {
      string path = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");
      Credentials = JObject.Parse(File.ReadAllText(path)).ToObject<Credentials>();
    }

    public override string ToString()
    {
      return $"Username: {Credentials.Username}; ClientID: {Credentials.ClientID}";
    }
  }
}
