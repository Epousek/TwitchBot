using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.src
{
  public static class BotInfo
  {
    public static DateTime RunningSince { get; set; }
    public static int CommandsUsedSinceStart { get; set; }
    public static int MessagesLoggedSinceStart { get; set; }
  }
}
