using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Humanizer.Localisation;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands.Status
{
  public class CheckStatus : ICommand
  {
    public string Name { get; } = "Status";
    public string AboutCommand { get; } = "Zjistíš jaký status má momentálně daný uživatel.";
    public string HelpMessage { get; } = "$status *uživatel*";
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
      var comArgs = new CommandArguments(message);
      if((comArgs.GetOneArgument().Count == 0) || string.IsNullOrEmpty(message.Message))
      {
        Bot.WriteMessage($"@{message.Username} musíš zadat username uživatele.", message.Channel);
        return;
      }

      var username = comArgs.GetOneArgument()[0];
      var status = await DatabaseConnections.GetUserStatus(message.Channel, username).ConfigureAwait(false);

      if (status == null)
      {
        Bot.WriteMessage($"@{message.Username} tohoto uživatele nemám v databázi :/", message.Channel);
      }
      else if (status.Status == Enums.Status.None)
      {
        Bot.WriteMessage($"@{message.Username} tento uživatel nemá žádný status.", message.Channel);
      }
      else if (status.Status == Enums.Status.Afk)
      {
        var builder = new StringBuilder();
        builder
          .Append('@')
          .Append(message.Username)
          .Append(" tento uživatel je momentálně afk")
          .Append(string.IsNullOrEmpty(status.Message) ? ". " : $": {status.Message} ")
          .Append('(')
          .Append((DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS")))
          .Append(')');
        Bot.WriteMessage(builder.ToString(), message.Channel);
      }
      else if (status.Status == Enums.Status.Gn)
      {
        var builder = new StringBuilder();
        builder
          .Append('@')
          .Append(message.Username)
          .Append(" tento uživatel momentálně spí")
          .Append(string.IsNullOrEmpty(status.Message) ? ". " : $": {status.Message} ")
          .Append('(')
          .Append((DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS")))
          .Append(')');
        Bot.WriteMessage(builder.ToString(), message.Channel);
      }
      else if (status.Status == Enums.Status.Food)
      {
        var builder = new StringBuilder();
        builder
          .Append('@')
          .Append(message.Username)
          .Append(" tento uživatel momentálně jí")
          .Append(string.IsNullOrEmpty(status.Message) ? ". " : $": {status.Message} ")
          .Append('(')
          .Append((DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS")))
          .Append(')');
        Bot.WriteMessage(builder.ToString(), message.Channel);
      }
      else if (status.Status == Enums.Status.School)
      {
        var builder = new StringBuilder();
        builder
          .Append('@')
          .Append(message.Username)
          .Append(" tento uživatel se momentálně učí")
          .Append(string.IsNullOrEmpty(status.Message) ? ". " : $": {status.Message} ")
          .Append('(')
          .Append((DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS")))
          .Append(')');
        Bot.WriteMessage(builder.ToString(), message.Channel);
      }
      else if (status.Status == Enums.Status.Work)
      {
        var builder = new StringBuilder();
        builder
          .Append('@')
          .Append(message.Username)
          .Append(" tento uživatel momentálně pracuje")
          .Append(string.IsNullOrEmpty(status.Message) ? ". " : $": {status.Message} ")
          .Append('(')
          .Append((DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS")))
          .Append(')');
        Bot.WriteMessage(builder.ToString(), message.Channel);
      }
    }
  }
}

