using System;
using System.Threading.Tasks;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands
{
  internal class Unadmin : ICommand
  {
    public string Name { get; } = nameof(Unadmin);
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

      if (await DatabaseConnections.GetPermission(message.Channel, args[0]).ConfigureAwait(false) != Permission.Admin)
      {
        Bot.WriteMessage("@" + message.Username + " Uživatel není admin.", message.Channel);
      }
      else
      {
        await DatabaseConnections.UpdateUser("perms", message.Channel, args[0], permission: Permission.Regular).ConfigureAwait(false);
        Bot.WriteMessage("@" + message.Username + " " + args[0] + " už není admin na tomto kanále.", message.Channel);
      }
    }
  }
}
