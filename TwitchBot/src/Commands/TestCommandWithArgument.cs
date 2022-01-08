using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Enums;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  class TestCommandWithArgument : ICommand
  {
    public string Name { get; } = nameof(Emotes);
    public string AboutCommand { get; } = "Vypíše naposledy přidané emoty";
    public string Help { get; } = "$emotes";
    public string[] Aliases { get; } = { "emotikony", "added" };
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
      var ca = new CommandArguments(message);
      if(ca.GetTwoArguments().Count >= 2)
      {
        if (!string.IsNullOrEmpty(ca.GetTwoArguments()[0]) && !string.IsNullOrEmpty(ca.GetTwoArguments()[1]))
          Bot.WriteMessage($"1: {ca.GetTwoArguments()[0]}; 2: {ca.GetTwoArguments()[1]}", message.Channel);
      }
      else
      {
        Bot.WriteMessage("musíš napsat dva argumenty :/", message.Channel);
      }
    }
  }
}
