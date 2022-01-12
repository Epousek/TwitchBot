using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Enums;
using TwitchBot.src.Models;
using TwitchBot.src.Connections;
using TwitchBot.src.Interfaces;
using Humanizer;

namespace TwitchBot.src.Commands
{
  class Info : ICommand
  {
    public string Name { get; } = nameof(Info);
    public string AboutCommand { get; } = "Vypíše informace o právě běžící instanci bota.";
    public string HelpMessage { get; } = "$info";
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
      var release = await GitHub.GetLastReleaseAsync();

      StringBuilder sb = new StringBuilder("@");
      sb.Append(message.Username)
        .Append(" Uptime: ")
        .Append((DateTime.Now - BotInfo.RunningSince).Humanize(3, culture: new("cs-CS"), minUnit: Humanizer.Localisation.TimeUnit.Second))
        .Append("; počet použitých příkazů od zapnutí: ")
        .Append(BotInfo.CommandsUsedSinceStart)
        .Append("; verze: ")
        .Append(release.TagName[1..])
        .Append(" (")
        .Append(release.PublishedAt.Value.DateTime)
        .Append(')');

      Bot.WriteMessage(sb.ToString(), message.Channel);
    }
  }
}
