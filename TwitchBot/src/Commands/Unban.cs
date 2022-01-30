using System;
using System.Threading.Tasks;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands
{
  internal class Unban : ICommand
  {
    public string Name { get; } = nameof(Unban);
    public string AboutCommand { get; } = "Odbanuje daného uživatele.";
    public string HelpMessage { get; } = "$unban *username*";
    public string[] Aliases { get; } = Array.Empty<string>();
    public Permission Permission { get; } = Permission.Admin;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(0);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get; set; }

    public async Task UseCommandAsync(ChatMessageModel message)
    {
      var comArgs = new CommandArguments(message);
      var args = comArgs.GetOneArgument();
      if (args == null)
      {
        Bot.WriteMessage("@" + message.Username + " Zadej jeden argument.", message.Channel);
      }
      else
      {
        if (await DatabaseConnections.IsInUsers(message.Channel, args[0]).ConfigureAwait(false) && await DatabaseConnections.IsBanned(message.Channel, args[0]).ConfigureAwait(false))
        {
          await DatabaseConnections.UpdateUser("ban", message.Channel, args[0], false).ConfigureAwait(false);
          Bot.WriteMessage(args[0] + " může opět používat příkazy na tomto kanále.", message.Channel);
        }
        else
        {
          Bot.WriteMessage("@" + message.Username + " Tento uživatel nemá ban.", message.Channel);
        }
      }
    }
  }
}
