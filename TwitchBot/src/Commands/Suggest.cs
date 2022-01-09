using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Enums;
using TwitchBot.src.Models;
using TwitchBot.src.Connections;

namespace TwitchBot.src.Commands
{
  class Suggest : ICommand
  {
    public string Name { get; } = nameof(Suggest);
    public string AboutCommand { get; } = "Pomocí tohoto příkazu můžeš navrhnout novou funkci nebo nahlásit chybu.";
    public string HelpMessage { get; } = "$suggest *návrh*";
    public string[] Aliases { get; } = { "návrh" };
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
      message.Message = message.Message[message.Message.IndexOf(' ')..];
      await DatabaseConnections.WriteSuggestion(message);
      Bot.WriteMessage($"@{message.Username} díky za návrh!", message.Channel);
    }
  }
}
