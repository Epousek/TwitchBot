using System;
using System.Threading.Tasks;
using TwitchBot.Enums;
using TwitchBot.Models;

namespace TwitchBot.Interfaces
{
  public interface ICommand
  {
    string Name { get; }
    string AboutCommand { get; }
    string HelpMessage { get; }
    string[] Aliases { get; }
    Permission Permission { get; }
    TimeSpan Cooldown { get; }
    DateTime LastUsed { get; set; }
    bool OfflineOnly { get; }
    bool UsableByBanned { get; }
    bool Optoutable { get; }
    int TimesUsedSinceRestart { get; set; }
    int? TimesUsedTotal { get; set; }

    public Task UseCommandAsync(ChatMessageModel message);
  }
}
