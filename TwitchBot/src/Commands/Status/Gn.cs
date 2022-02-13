using System;
using System.Threading.Tasks;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands.Status
{
  public class Gn : ICommand
  {
    public string Name { get; } = nameof(Gn);
    public string AboutCommand { get; } = "Tvůj status se změní na spánek.";
    public string HelpMessage { get; } = "$gn *zpráva*";
    public string[] Aliases { get; } = Array.Empty<string>();
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
      if (await DatabaseConnections.GetStatusEnumOnly(message.Channel, message.Username).ConfigureAwait(false) == Enums.Status.Gn)
      {
        Bot.WriteMessage($"@{message.Username} už spíš.", message.Channel); //TODO: update status
        return;
      }

      var comArgs = new CommandArguments(message);
      var args = comArgs.GetOneArgument();

      var status = new StatusModel
      {
        Channel = message.Channel,
        Username = message.Username,
        StatusSince = DateTime.Now,
        Status = Enums.Status.Gn
      };

      if (args.Count == 0)
      {
        status.Message = "";
        Bot.WriteMessage($"@{message.Username} jde spát.", message.Channel);
      }
      else
      {
        status.Message = args[0];
        Bot.WriteMessage($"@{message.Username} jde spát: {args[0]}", message.Channel);
      }

      if (await DatabaseConnections.IsInUserStatuses(message.Channel, message.Username).ConfigureAwait(false))
        await DatabaseConnections.UpdateUserStatus(status).ConfigureAwait(false);
      else
        await DatabaseConnections.AddUserStatus(status).ConfigureAwait(false);
    }
  }
}
