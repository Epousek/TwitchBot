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
      if(args.Count > 0)
        args[0] = Helpers.FirstToUpper(args[0]);

      if (args.Count != 1)
      {
        Bot.WriteMessage($"@{message.Username} pro pomoc s příkazem napiš $help *příkaz*. Pro seznam příkazů napiš $commands.", message.Channel);
        return;
      }
      if(args[0].Contains(' '))
      {
        Bot.WriteMessage($"@{message.Username} napiš jenom název příkazu. :)", message.Channel);
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
          builder.Append("; místo ");

          if (command.HelpMessage.Contains(' '))
            builder.Append(command.HelpMessage, 0, command.HelpMessage.IndexOf(' '));
          else
            builder.Append(command.HelpMessage);
          
          builder.Append(" můžeš použít jeden z aliasů: ");

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
      else
      {
        var commands = commandInstances.Where(c =>
        {
          if (c.Value.Aliases.Length > 0)
          {
            foreach (string alias in c.Value.Aliases)
            {
              if (string.Equals(args[0], alias, StringComparison.OrdinalIgnoreCase))
                return true;
            }
          }
          return false;
        });

        if (commands?.Any() == true)
        {
          var command = commands.First().Value;
          var builder = new StringBuilder("@");

          builder
            .Append(message.Username)
            .Append(" Pro použití tohoto příkazu napiš: ")
            .Append(command.HelpMessage)
            .Append("; místo ");

          if (command.HelpMessage.Contains(' '))
            builder.Append(command.HelpMessage, 0, command.HelpMessage.IndexOf(' '));
          else
            builder.Append(command.HelpMessage);

          builder.Append(" můžeš použít jeden z aliasů: ");

          foreach (var alias in command.Aliases)
          {
            builder
              .Append(alias)
              .Append(", ");
          }

          builder
            .Remove(builder.Length - 2, 2)
            .Append('.');

          Bot.WriteMessage(builder.ToString(), message.Channel);
        }
        else
        {
          Bot.WriteMessage($"@{message.Username} tento příkaz buď neexistuje, nebo jsi ho napsal špatně, nebo epousek něco posral", message.Channel);
        }
      }
    }
  }
}
