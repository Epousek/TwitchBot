using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.src.Enums;
using TwitchBot.src.Interfaces;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  internal class Remind : ICommand
  {
    public string Name { get; } = nameof(Remind);
    public string AboutCommand { get; } = "Upozorní daného uživatele na něco za danou dobu/až příště napíše do chatu.";
    public string HelpMessage { get; } = "Časované upozornění: $remind *username/me* in *čas(např. 60m, 120s...)* *zpráva*; upozornění po napsání do chatu: $remind *username* *zpráva*";
    public string[] Aliases { get; } = Array.Empty<string>();
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(10);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public async Task UseCommandAsync(ChatMessageModel message)
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
            reminder.For = args[0];
            reminder.From = message.Username;
            reminder.Channel = message.Channel;
          }
        }
        {

        }
      }
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
