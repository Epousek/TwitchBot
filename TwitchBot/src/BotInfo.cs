using System;

namespace TwitchBot
{
  public static class BotInfo
  {
    public static DateTime RunningSince { get; set; }
    public static int CommandsUsedSinceStart { get; set; }
    public static int MessagesLoggedSinceStart { get; set; }
  }
}
