﻿using System;
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
  internal class Optin : ICommand
  {
    public string Name { get; } = nameof(Optout);
    public string AboutCommand { get; } = "Zrušíš optout na daný příkaz. Zatím funguje pro: remind";
    public string HelpMessage { get; } = "$optin *příkaz*";
    public string[] Aliases { get; } = Array.Empty<string>();
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(5);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = true;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public async Task UseCommandAsync(ChatMessageModel message)
    {
      var comArgs = new CommandArguments(message);
      var args = comArgs.GetOneArgument();

      if (args.Count == 0)
      {
        Bot.WriteMessage($"@{message.Username} Zadej příkaz. :)", message.Channel);
      }
      else if (Bot.cg.commandInstances.Any(x => string.Equals(args[0], x.Key, StringComparison.OrdinalIgnoreCase) && x.Value.Optoutable))
      {
        if (!await DatabaseConnections.IsInUsers(message.Channel, message.Username).ConfigureAwait(false))
          await DatabaseConnections.WriteToUsers(message.Channel, message.Username).ConfigureAwait(false);

        await DatabaseConnections.UpdateOptout(message.Channel, message.Username, args[0].ToLower(), false).ConfigureAwait(false);
        Bot.WriteMessage($"@{message.Username} Úspěšně jsi zrušil optout pro {args[0]}. (na tomto kanále)", message.Channel);
      }
      else
      {
        Bot.WriteMessage($"@{message.Username} Tento příkaz neexistuje nebo z něj nejde optoutnout.", message.Channel);
      }
    }
  }
}
