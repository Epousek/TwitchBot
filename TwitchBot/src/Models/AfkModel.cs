using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.src.Models
{
  public class AfkModel
  {
    public string Channel { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
    public DateTime AfkSince { get; set; }
    public bool? IsAfk { get; set; }
  }
}
