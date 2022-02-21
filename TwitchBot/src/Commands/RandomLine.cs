using System;
using System.Threading.Tasks;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands;

public class RandomLine : ICommand
{
  public string Name { get; } = "Randomline";
  public string AboutCommand { get; } = "Vypíše náhodnou zprávu z logů tohoto kanálu. Lze specifikovat uživatel.";
  public string HelpMessage { get; } = "$rl *uživatel*";
  public string[] Aliases { get; } = { "rl" };
  public Permission Permission { get; } = Permission.Regular;
  public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(1);
  public DateTime LastUsed { get; set; }
  public bool OfflineOnly { get; } = false;
  public bool UsableByBanned { get; } = false;
  public bool Optoutable { get; } = false;
  public int TimesUsedSinceRestart { get; set; }
  public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

  public async Task UseCommandAsync(ChatMessageModel message)
  {
    var comArgs = new CommandArguments(message);
    var args = comArgs.GetOneArgument();

    if (args.Count > 0)
    {
      var randomLine = await DatabaseConnections.GetRandomLine(message.Channel, args[0]);
      Bot.WriteMessage(
        randomLine == null
          ? $"@{message.Username} Uživatel nebyl nalezen. :/"
          : $"@{message.Username} {randomLine.Username}: {randomLine.Message} ({randomLine.TimeStamp})",
        message.Channel);
    }
    else
    {
      var randomLine = await DatabaseConnections.GetRandomLine(message.Channel).ConfigureAwait(false);
      Bot.WriteMessage(
        randomLine == null
          ? $"@{message.Username} Nastala chyba při komunikaci s databází. :/ (@epousek)"
          : $"@{message.Username} {randomLine.Username}: {randomLine.Message} ({randomLine.TimeStamp})",
        message.Channel);
    }
  }
}