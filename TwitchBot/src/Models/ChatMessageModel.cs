using System;

namespace TwitchBot.Models
{
  public class ChatMessageModel
  {
    public string Channel { get; init; }
    public string Username { get; init; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; init; }
  }
}
