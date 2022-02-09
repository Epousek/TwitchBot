using System;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands
{
  internal class Commands : ICommand
  {
    public string Name { get; } = nameof(Commands);
    public string AboutCommand { get; } = "Vypíše seznam všech příkazů";
    public string HelpMessage { get; } = "$commands";
    public string[] Aliases { get; } = { "příkazy" };
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(20);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Task UseCommandAsync(ChatMessageModel message)
    {
      var builder = new StringBuilder("@");
      builder.Append(message.Username)
        .Append(" seznam všech příkazů: ");
      foreach (var command in Bot.CmdGetter.CommandInstances)
      {
        builder.Append(command.Key.ToLower())
          .Append(", ");
      }
      builder.Remove(builder.Length - 2, 2)
        .Append('.');

      Bot.WriteMessage(builder.ToString(), message.Channel);
      return Task.CompletedTask;
    }
  }
}
