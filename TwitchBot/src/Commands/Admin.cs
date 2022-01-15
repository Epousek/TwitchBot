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
  internal class Admin : ICommand
  {
    public string Name { get; } = nameof(Admin);
    public string AboutCommand { get; } = "Dá danému uživateli oprávnění admina.";
    public string HelpMessage { get; } = "$admin *username*";
    public string[] Aliases { get; } = Array.Empty<string>();
    public Permission Permission { get; } = Permission.Owner;
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

      if (await DatabaseConnections.GetPermission(message.Channel, args[0]).ConfigureAwait(false) == Permission.Admin)
      {
        Bot.WriteMessage("@" + message.Username + " Uživatel už je admin.", message.Channel);
      }
      else
      {
        await DatabaseConnections.UpdateUser("perms", message.Channel, args[0], permission: Permission.Admin).ConfigureAwait(false);
        Bot.WriteMessage("@" + message.Username + " " + args[0] + " je nyní admin na tomto kanále.", message.Channel);
      }
    }
  }
}
