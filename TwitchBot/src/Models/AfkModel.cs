using System;

namespace TwitchBot.Models
{
  public class AfkModel
  {
    public string Channel { get; init; }
    public string Username { get; init; }
    public string Message { get; set; }
    public DateTime AfkSince { get; set; }
    public bool? IsAfk { get; set; }
  }
}
