using System;

namespace TwitchBot.src.Models
{
  public class ChatMessageModel
  {
    public string Channel { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
  }
}
