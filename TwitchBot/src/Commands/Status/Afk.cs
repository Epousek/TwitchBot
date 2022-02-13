using System;
using System.Globalization;
using System.Threading.Tasks;
using Humanizer;
using Humanizer.Localisation;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace TwitchBot.Commands
{
  internal class Afk : ICommand
  {
    public string Name { get; } = nameof(Afk);
    public string AboutCommand { get; } = "Budeš označen jako AFK.";
    public string HelpMessage { get; } = "$afk *zpráva*";
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
      if (await DatabaseConnections.IsAfk(message.Channel, message.Username).ConfigureAwait(false))
      {
        Bot.WriteMessage($"@{message.Username} už jsi afk.", message.Channel);
        return;
      }

      var comArgs = new CommandArguments(message);
      var args = comArgs.GetOneArgument();

      var afk = new StatusModel
      {
        Channel = message.Channel,
        Username = message.Username,
        StatusSince = DateTime.Now,
        IsAfk = true
      };

      if (args.Count == 0)
      {
        afk.Message = "";
        Bot.WriteMessage($"@{message.Username} je nyní afk.", message.Channel);
      }
      else
      {
        afk.Message = args[0];
        Bot.WriteMessage($"@{message.Username} je nyní afk: {args[0]}", message.Channel);
      }

      if (await DatabaseConnections.IsInAfkUsers(message.Channel, message.Username).ConfigureAwait(false))
      {
        await DatabaseConnections.UpdateAfkUser(afk).ConfigureAwait(false);
      }
      else
      {
        await DatabaseConnections.AddAfkUser(afk).ConfigureAwait(false);
      }
    }

    public static async Task CheckAfk(ChatMessageModel message)
    {
      if (message.Message.StartsWith($"{Bot.Prefix}afk", StringComparison.OrdinalIgnoreCase))
        return;

      if (await DatabaseConnections.IsAfk(message.Channel, message.Username))
      {
        var afk = await DatabaseConnections.GetAfkUser(message.Channel, message.Username).ConfigureAwait(false);

        if (string.IsNullOrEmpty(afk.Message))
          Bot.WriteMessage($"{afk.Username} už není afk! ({(DateTime.Now - afk.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})", afk.Channel);
        else
          Bot.WriteMessage($"{afk.Username} už není afk: {afk.Message} ({(DateTime.Now - afk.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})", afk.Channel);

        afk.IsAfk = false;
        await DatabaseConnections.UpdateAfkUser(afk).ConfigureAwait(false);
      }
    }
  }
}
