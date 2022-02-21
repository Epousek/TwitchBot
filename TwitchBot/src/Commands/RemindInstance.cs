using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Humanizer.Localisation;
using TwitchBot.Connections;
using TwitchBot.Models;

namespace TwitchBot.Commands
{

  public class RemindInstance
  {
    public static List<Reminder> Reminders { get; } = new List<Reminder>();

    public async Task NewReminder(ChatMessageModel message)
    {
      var comArgs = new CommandArguments(message);
      var args = comArgs.GetXArguments(3);

      if (args != null && await DatabaseConnections.CheckOptout(message.Channel, args[0], "remind").ConfigureAwait(false))
      {
        Bot.WriteMessage($"@{message.Username} tomuto uživateli nelze poslat připomenutí. :/", message.Channel);
        return;
      }

      if (!string.IsNullOrEmpty(args[1]))
      {
        if (args[1] == "in")
        {   //TIMED REMIND
          args = comArgs.GetXArguments(4);
          CheckTime(args, message, out var reminder);
          if (reminder == null) 
            return;
          if (args[0].StartsWith('@'))
            args[0] = args[0].Replace("@", "");
          reminder.IsTimed = true;
          reminder.From = message.Username;
          reminder.For = string.Equals(args[0], "me", StringComparison.OrdinalIgnoreCase) ? reminder.From : args[0];
          reminder.Channel = message.Channel;
          reminder.Message = string.IsNullOrEmpty(args[3]) ? string.Empty : args[3];

          if (await CheckIfAlreadyReminding(reminder).ConfigureAwait(false))
          {
            Bot.WriteMessage($"@{reminder.From} Pro tohoto uživatele už máš upozornění :/", reminder.Channel);
            return;
          }

          _ = new Timer(TimerElapsed, reminder, (int)reminder.Length.Value.TotalMilliseconds, Timeout.Infinite);

          await DatabaseConnections.AddReminder(reminder).ConfigureAwait(false);
          reminder.Id = await DatabaseConnections.GetReminderId(reminder).ConfigureAwait(false);

          var builder = new StringBuilder("@");
          builder
            .Append(reminder.From)
            .Append(' ');
          if (string.Equals(reminder.For, reminder.From, StringComparison.OrdinalIgnoreCase))
          {
            builder
              .Append("Budeš upozorněn(a) za ")
              .Append(reminder.Length.Value.Humanize(3, new CultureInfo("cs-CS"), minUnit: TimeUnit.Second));
          }
          else
          {
            builder
              .Append(reminder.For)
              .Append(" bude upozorněn(a) za ")
              .Append(reminder.Length.Value.Humanize(3, new CultureInfo("cs-CS"), minUnit: TimeUnit.Second));
          }
          Bot.WriteMessage(builder.ToString(), message.Channel);
          return;

        }
      }
      if (!string.IsNullOrEmpty(args[0]))
      {
        args = comArgs.GetXArguments(2);

        var reminder = new Reminder
        {
          IsTimed = false,
          From = message.Username,
          For = args[0],
          StartTime = DateTime.Now,
          Channel = message.Channel,
          Message = string.IsNullOrEmpty(args[1]) ? string.Empty : args[1],
          Length = null
        };
        reminder.For = string.Equals(reminder.For, "me") ? reminder.From : reminder.For;

        if (await CheckIfAlreadyReminding(reminder).ConfigureAwait(false))
        {
          Bot.WriteMessage($"@{reminder.From} Pro tohoto uživatele už máš upozornění :/", reminder.Channel);
          return;
        }

        await DatabaseConnections.AddReminder(reminder).ConfigureAwait(false);
        reminder.Id = await DatabaseConnections.GetReminderId(reminder).ConfigureAwait(false);

        var builder = new StringBuilder("@");
        builder
          .Append(reminder.From)
          .Append(' ');
        if (string.Equals(reminder.For, reminder.From, StringComparison.OrdinalIgnoreCase))
        {
          builder.Append("Budeš upozorněn(a) až příště napíšeš do chatu.");
        }
        else
        {
          builder
            .Append(reminder.For)
            .Append(" bude upozorněn(a) až příště něco napíše do chatu.");
        }
        Bot.WriteMessage(builder.ToString(), reminder.Channel);
        Reminders.Add(reminder);
      }
    }

    private static async void TimerElapsed(object state)
    {
      var reminder = (Reminder)state;
      var builder = new StringBuilder("@");
      builder.Append(reminder.For);
      if (string.Equals(reminder.From, reminder.For, StringComparison.OrdinalIgnoreCase))
      {
        builder.Append(" Upozornění od tebe");
      }
      else
      {
        builder
          .Append(" Upozornění od ")
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
      await DatabaseConnections.DeactivateReminder(reminder).ConfigureAwait(false);
    }


    private static void CheckTime(IList<string> args, ChatMessageModel message, out Reminder reminder)
    {
      reminder = new Reminder();
      if (string.IsNullOrEmpty(args[2]))
      {     //NO TIME ARG
        reminder = null;
        Bot.WriteMessage($"@{message.Username} Musíš zadat čas, za jak dlouho mám uživatele upozornit (např. 30s; 5m; 1h...)", message.Channel);
      }
      else
      {
        try
        {     //no units specified => minutes
          var time = int.Parse(args[2]);
          reminder.StartTime = DateTime.Now;
          reminder.Length = TimeSpan.FromMinutes(time);
          reminder.EndTime = reminder.StartTime + reminder.Length;
        }
        catch
        {
          if (Regex.IsMatch(args[2], "^[1-9]{1}[0-9]{0,4}[sS,mM,hH]$"))
          {
            if (args[2].Contains("s", StringComparison.OrdinalIgnoreCase))
            {
              args[2] = args[2].Remove(args[2].IndexOf("s", StringComparison.OrdinalIgnoreCase));
              reminder.StartTime = DateTime.Now;
              reminder.Length = TimeSpan.FromSeconds(int.Parse(args[2]));
              reminder.EndTime = reminder.StartTime + reminder.Length;
            }
            else if (args[2].Contains("m", StringComparison.OrdinalIgnoreCase))
            {
              args[2] = args[2].Remove(args[2].IndexOf("m", StringComparison.OrdinalIgnoreCase));
              reminder.StartTime = DateTime.Now;
              reminder.Length = TimeSpan.FromMinutes(int.Parse(args[2]));
              reminder.EndTime = reminder.StartTime + reminder.Length;
            }
            else if (args[2].Contains("h", StringComparison.OrdinalIgnoreCase))
            {
              args[2] = args[2].Remove(args[2].IndexOf("h", StringComparison.OrdinalIgnoreCase));
              reminder.StartTime = DateTime.Now;
              reminder.Length = TimeSpan.FromHours(int.Parse(args[2]));
              reminder.EndTime = reminder.StartTime + reminder.Length;
            }
            else
            {
              reminder = null;
              Bot.WriteMessage("xd", message.Channel);
            }
          }
          else
          {
            reminder = null;
            Bot.WriteMessage($"@{message.Username} Musíš napsat čas ve formátu *číslo**jednotka* (např. 30s; 5m; 1h...)", message.Channel);
          }
        }
      }
    }

    private static async Task<bool> CheckIfAlreadyReminding(Reminder reminder)
      => await DatabaseConnections.AlreadyReminding(reminder).ConfigureAwait(false);
  }
}
