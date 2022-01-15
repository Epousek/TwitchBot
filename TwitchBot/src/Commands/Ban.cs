using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Connections;
using TwitchBot.src.Enums;
using TwitchBot.src.Interfaces;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  internal class Ban : ICommand
  {
    public string Name { get; } = nameof(Ban);
    public string AboutCommand { get; } = "Zabanuje daného uživatele.";
    public string HelpMessage { get; } = "$ban *username*";
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
      Permission? perms = await DatabaseConnections.GetPermission(message.Channel, args[0]).ConfigureAwait(false);

      if (perms != null && perms != Permission.Regular)
      {
        Bot.WriteMessage("@" + message.Username + " Nemůžeš zabanovat admina/ownera. :/", message.Channel);
        return;
      }
      if (!await DatabaseConnections.IsInUsers(message.Channel, args[0]).ConfigureAwait(false))
      {
        await DatabaseConnections.WriteToUsers(message.Channel, args[0]).ConfigureAwait(false);
        await DatabaseConnections.UpdateUser("ban", message.Channel, args[0], true).ConfigureAwait(false);
      }
      else
      {
        await DatabaseConnections.UpdateUser("ban", message.Channel, args[0], true).ConfigureAwait(false);
      }
      Bot.WriteMessage(args[0] + " nyní nemůže používat příkazy na tomto kanále.", message.Channel);
    }
  }
}
