using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Enums;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  class Commands : ICommand
  {
    public string Name { get; } = nameof(Commands);
    public string AboutCommand { get; } = "Vypíše seznam všech příkazů";
    public string Help { get; } = "prostě napiš ten příkaz lol";
    public string[] Aliases { get; } = { "příkazy" };
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(20);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public async Task UseCommandAsync(ChatMessageModel message)
    {
      StringBuilder sb = new StringBuilder("@");
      sb.Append(message.Username)
        .Append(" seznam všech příkazů: ");
      foreach (var command in Bot.cg.commandInstances)
      {
        sb.Append(command.Key)
          .Append(", ");
      }
      sb.Remove(sb.Length - 2, 2)
        .Append('.');

      Bot.WriteMessage(sb.ToString(), message.Channel);
    }
  }
}
