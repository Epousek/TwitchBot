using Humanizer;
using Humanizer.Localisation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TwitchBot.src.Connections;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  public class RemindInstance
  {
    public async Task NewReminder(ChatMessageModel message)
    {
      var comArgs = new CommandArguments(message);
      var args = comArgs.GetXArguments(3);
      if (!string.IsNullOrEmpty(args[1]))
      {
        if (args[1] == "in")
        {   //TIMED REMIND
          args = comArgs.GetXArguments(4);
          Reminder reminder;
          CheckTime(args, out reminder);
          if (reminder != null)
          {
            reminder.IsTimed = true;
            reminder.From = message.Username;
            reminder.For = string.Equals(args[0], "me") ? reminder.From : args[0];
            reminder.Channel = message.Channel;
            reminder.Message = String.IsNullOrEmpty(args[3]) ? String.Empty : args[3];

            var timer = new Timer(TimerElapsed, reminder, (int)reminder.Length.Value.TotalMilliseconds, Timeout.Infinite);
            await DatabaseConnections.AddReminder(reminder).ConfigureAwait(false);
            reminder.ID = await DatabaseConnections.GetReminderID(reminder).ConfigureAwait(false);

            var builder = new StringBuilder('@');
            builder
              .Append(reminder.From)
              .Append(' ');
            if (string.Equals(reminder.For, reminder.From, StringComparison.OrdinalIgnoreCase))
            {
              builder
                .Append("budeš upozorněn za ")
                .Append(reminder.Length.Value.Humanize(3, new CultureInfo("cs-CS"), TimeUnit.Second));
            }
            else
            {
              builder
                .Append(reminder.For)
                .Append(" bude upozorněn za ")
                .Append(reminder.Length.Value.Humanize(3, new CultureInfo("cs-CS"), TimeUnit.Second));
            }
            Bot.WriteMessage(builder.ToString(), message.Channel);
          }
        }
        {

        }
      }
    }

    private async void TimerElapsed(object state)
    {
      var reminder = (Reminder)state;
      var builder = new StringBuilder("@");
      if (string.Equals(reminder.From, reminder.For, StringComparison.OrdinalIgnoreCase))
      {
        builder
          .Append(reminder.For)
          .Append(" Upozornění od tebe");
      }
      else
      {
        builder
          .Append(reminder.For)
          .Append(" Upozornění od ")
          .Append(reminder.From);
      }
      if (string.IsNullOrEmpty(reminder.Message))
      {
        builder.Append(" bez zprávy. ");
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

    private void CheckTime(List<string> args, out Reminder reminder)
    {
      reminder = new Reminder();
      if (string.IsNullOrEmpty(args[2]))
      {     //NO TIME ARG
        reminder = null;
      }
      else
      {
        int time;
        try
        {     //no units specified => minutes
          time = int.Parse(args[2]);
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
            }
          }
        }
      }
    }
  }
}
