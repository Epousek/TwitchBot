using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Enums;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  class Help : ICommand
  {
    public string Name { get; } = nameof(Emotes);
    public string AboutCommand { get; } = "Napíše jak použít daný command.";
    public string HelpMessage { get; } = "$help *název příkazu*";
    public string[] Aliases { get; } = { "pomoc" };
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(5);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public async Task UseCommandAsync(ChatMessageModel message)
    {
      var ca = new CommandArguments(message);
      var args = ca.GetOneArgument();
      var commandInstances = Bot.cg.commandInstances;
      args[0] = Helpers.FirstToUpper(args[0]);

      if (args.Count != 1)
      {
        Bot.WriteMessage($"@{message.Username} zadej prosím jeden argument", message.Channel);
        return;
      }
      if (commandInstances.ContainsKey(args[0]))
      {
        var command = commandInstances[args[0]];
        var builder = new StringBuilder("@");

        builder
          .Append(message.Username)
          .Append(" Pro použití tohoto příkazu napiš: ")
          .Append(command.HelpMessage);

        if (command.Aliases.Length > 0)
        {
          builder.Append("; příkaz má následující aliasy: ");
          foreach (var alias in command.Aliases)
          {
            builder
              .Append(alias)
              .Append(", ");
          }
          builder
            .Remove(builder.Length - 2, 2)
            .Append('.');
        }

        Bot.WriteMessage(builder.ToString(), message.Channel);
      }
    }
  }
}
