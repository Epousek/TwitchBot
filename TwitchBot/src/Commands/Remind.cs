﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using TwitchBot.src.Connections;
using TwitchBot.src.Enums;
using TwitchBot.src.Interfaces;
using TwitchBot.src.Models;
using System.Text;
using Humanizer;
using System.Globalization;
using Humanizer.Localisation;

namespace TwitchBot.src.Commands
{
  internal class Remind : ICommand
  {
    public string Name { get; } = nameof(Remind);
    public string AboutCommand { get; } = "Upozorní daného uživatele na něco za danou dobu/až příště napíše do chatu.";
    public string HelpMessage { get; } = "Časované upozornění: $remind *username/me* in *čas(např. 60m, 120s...)* *zpráva*; upozornění po napsání do chatu: $remind *username* *zpráva*";
    public string[] Aliases { get; } = Array.Empty<string>();
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(0);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public List<Remind> Reminders { get; }

    public async Task UseCommandAsync(ChatMessageModel message)
    {
      _ = new RemindInstance().NewReminder(message);
    }

    public async Task CheckForReminder(ChatMessageModel message)
    {
      if (RemindInstance.Reminders.Any(x => string.Equals(x.For, message.Username, StringComparison.OrdinalIgnoreCase)))
      {
        var reminder = RemindInstance.Reminders.Where(x => string.Equals(x.For, message.Username, StringComparison.OrdinalIgnoreCase)).ToArray()[0];
        var builder = new StringBuilder("@");
        builder.Append(reminder.For);
        if (string.Equals(reminder.For, reminder.From, StringComparison.OrdinalIgnoreCase))
        {
          builder.Append(" upozornění od tebe");
        }
        else
        {
          builder
            .Append(" upozornění od ")
            .Append(reminder.From);
        }
        if (string.IsNullOrEmpty(reminder.Message))
        {
          builder.Append("! (bez zprávy) ");
        }
        else
        {
          builder
            .Append(": ")
            .Append(reminder.Message)
            .Append(' ');
        }
        builder
          .Append('(')
          .Append((DateTime.Now - reminder.StartTime).Humanize(3, new CultureInfo("cs-CS"), minUnit: TimeUnit.Second))
          .Append(')');

        Bot.WriteMessage(builder.ToString(), reminder.Channel);
        RemindInstance.Reminders.Remove(reminder);
        await DatabaseConnections.DeactivateReminder(reminder).ConfigureAwait(false);
      }
    }
  }
}
