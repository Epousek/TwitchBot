using System;
using TwitchBot.Enums;

namespace TwitchBot.Models
{
  public class StatusModel
  {
    public string Channel { get; init; }
    public string Username { get; init; }
    public string Message { get; set; }
    public DateTime StatusSince { get; set; }
    public Status CurrentStatus { get; set; }
    public Status LastStatus { get; set; }
  }
}
