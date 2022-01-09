using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Enums;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  class About : ICommand
  {
    public string Name { get; } = nameof(Commands);
    public string AboutCommand { get; } = "Vypíše seznam všech příkazů";
    public string HelpMessage { get; } = "prostě napiš ten příkaz lol";
    public string[] Aliases { get; } = { "příkazy" };
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(20);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get; set; }

    public async Task UseCommandAsync(ChatMessageModel message)
    {
      var sb = new StringBuilder("@");
      sb.Append(message.Username)
        .Append(" Tento bot byl vytvořený původně jakožto maturitní práce. Pro seznam příkazů napiš $commands. Pro pomoc s jednotlivým příkazem napiš $help a název příkazu (TBD).");
      Bot.WriteMessage(sb.ToString(), message.Channel);
    }
  }
}
