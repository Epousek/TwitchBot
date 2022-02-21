using System;
using System.Threading.Tasks;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands.Status;

public class Rafk : ICommand
{
  public string Name { get; } = nameof(Rafk);
  public string AboutCommand { get; } = "Pokračuj v posledním statusu.";
  public string HelpMessage { get; } = "$rafk";
  public string[] Aliases { get; } = { "cafk" };
  public Permission Permission { get; } = Permission.Regular;
  public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(3);
  public DateTime LastUsed { get; set; }
  public bool OfflineOnly { get; } = false;
  public bool UsableByBanned { get; } = false;
  public bool Optoutable { get; } = false;
  public int TimesUsedSinceRestart { get; set; }
  public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

  public async Task UseCommandAsync(ChatMessageModel message)
  {
    var status = await DatabaseConnections.GetUserStatus(message.Channel, message.Username).ConfigureAwait(false);
    if (status == null)
    {
      Bot.WriteMessage($"@{message.Username} Nemáš status ve kterém bys mohl pokračovat.", message.Channel);
      return;
    }
    if (status.CurrentStatus != Enums.Status.None)
    {
      Bot.WriteMessage($"@{message.Username} Už máš status. (tímhle se ti nezrušil)", message.Channel);
      return;
    }
    if (status.LastStatus == Enums.Status.None)
    {
      Bot.WriteMessage($"@{message.Username} Nemáš status ve kterém bys mohl pokračovat.", message.Channel);
      return;
    }

    status.CurrentStatus = status.LastStatus;
    await DatabaseConnections.UpdateUserStatus(status);
    Bot.WriteMessage($"@{message.Username} Pokračuješ ve tvém posledím statusu.", message.Channel);
  }
}