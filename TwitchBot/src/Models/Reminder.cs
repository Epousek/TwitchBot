using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.src.Models
{
  public class Reminder
  {
    public string Channel { get; set; }
    public string From { get; set; }
    public string For { get; set; }
    public bool IsTimed { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Length { get; set; }
    public int? ID { get; set; }
  }
}
