using System;

namespace TwitchBot.Models
{
  public class Reminder
  {
    public string Channel { get; set; }
    public string From { get; set; }
    public string For { get; set; }
    public string Message { get; set; }
    public bool IsTimed { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Length { get; set; }
    public int? Id { get; set; }
  }
}
