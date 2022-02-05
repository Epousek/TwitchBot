using System;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands
{
  internal class About : ICommand
  {
    public string Name { get; } = nameof(About);
    public string AboutCommand { get; } = "Napíše základní informace o botovi";
    public string HelpMessage { get; } = "$about";
    public string[] Aliases { get; } = Array.Empty<string>();
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(20);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get; set; }

    public Task UseCommandAsync(ChatMessageModel message)
    {
      var sb = new StringBuilder("@");
      sb.Append(message.Username)
        .Append(" Tento bot byl vytvořený původně jakožto maturitní práce. Pro seznam příkazů napiš $commands. Pro pomoc s jednotlivým příkazem napiš $help a název příkazu.");
      Bot.WriteMessage(sb.ToString(), message.Channel);
      return Task.CompletedTask;
    }
  }
}
