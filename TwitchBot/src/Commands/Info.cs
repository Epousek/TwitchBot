using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Octokit;
using TwitchBot.Connections;
using TwitchBot.Interfaces;
using TwitchBot.Models;
using Permission = TwitchBot.Enums.Permission;

namespace TwitchBot.Commands
{
  internal class Info : ICommand
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
      var totalCommandsUsed = await DatabaseConnections.GetTotalCommandsUsed().ConfigureAwait(false);
      var builder = new StringBuilder("@");
      builder.Append(message.Username)
        .Append(" Uptime: ")
        .Append((DateTime.Now - BotInfo.RunningSince).Humanize(
          3,
          new CultureInfo("cs-CS"),
          minUnit: Humanizer.Localisation.TimeUnit.Second))
        .Append("; počet použitých příkazů od zapnutí: ")
        .Append(BotInfo.CommandsUsedSinceStart)
        .Append(totalCommandsUsed == -1 ? "" : $"; počet použitých příkazů celkem: {totalCommandsUsed}")
        .Append("; verze: ")
        .Append(release.TagName[1..])
        .Append(" (")
        .Append(release.PublishedAt.Value.DateTime.AddHours(1))
        .Append(')');

      Bot.WriteMessage(builder.ToString(), message.Channel);
    }
  }
}
