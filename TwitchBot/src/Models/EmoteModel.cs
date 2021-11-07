using System;
using Newtonsoft.Json;

namespace TwitchBot.src.Models
{
  public class EmoteModel
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("code")]
    private string Code { set { Name = value; } }
    public string Service { get; set; }
    public DateTime Added { get; set; }
    public DateTime Removed { get; set; }
    public bool IsActive { get; set; }
  }
}