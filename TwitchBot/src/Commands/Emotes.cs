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

namespace TwitchBot.Commands
{
  internal class Emotes : ICommand
  {
    public string Name { get; } = nameof(Emotes);
    public string AboutCommand { get; } = "Vypíše naposledy přidané emoty";
    public string HelpMessage { get; } = "$emotes";
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
      var emotes = await DatabaseConnections.GetLastAddedEmotes(message.Channel);
      var builder = new StringBuilder("@");
      builder.Append(message.Username);
      if (emotes.Count == 0)
      {
        builder.Append(" Od doby co jsem na tomto kanále nebyly přidány žádne emoty.");
        Bot.WriteMessage(builder.ToString(), message.Channel);
        return;
      }
      builder.Append(" Naposledy přidané emoty: ");

      for (int i = 0; i < emotes.Count; i++)
      {
        var dateTime = emotes[i].Added;
        if (dateTime == null)
          continue;

        var sinceAddition = (TimeSpan)(DateTime.Now - dateTime);

        builder.Append(emotes[i].Name);
        builder.Append(" (");
        builder.Append(sinceAddition.Humanize(3, minUnit: TimeUnit.Minute, culture: new CultureInfo("cs-CS")));
        builder.Append(i != emotes.Count - 1 ? "), " : ").");

      }

      Bot.WriteMessage(builder.ToString(), message.Channel);
    }
  }
}
