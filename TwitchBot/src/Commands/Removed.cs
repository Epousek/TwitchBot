using Humanizer;
using Humanizer.Localisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Connections;
using TwitchBot.src.Enums;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  class Removed : ICommand
  {
    public string Name { get; } = nameof(Removed);
    public string About { get; } = "Vypíše naposledy odebrané emoty na tomto kanále.";
    public string Help { get; } = "$removed";
    public string[] Aliases { get; } = { "odstraněné" };
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
      if (DateTime.Now - LastUsed < Cooldown)
        return;

      TimeSpan sinceAddition;
      List<EmoteModel> emotes = await DatabaseConnections.GetLastRemovedEmotes(message.Channel);
      StringBuilder builder = new("@");
      builder.Append(message.Username);
      if (emotes.Count == 0)
      {
        builder.Append(" Od doby co jsem na tomto kanále nebyly odebrány žádné emoty");
        Bot.WriteMessage(builder.ToString(), message.Channel);
        return;
      }
      builder.Append(" Naposledy odebrané emoty: ");

      for (int i = 0; i < emotes.Count; i++)
      {
        sinceAddition = (TimeSpan)(DateTime.Now - emotes[i].Removed);

        builder.Append(emotes[i].Name);
        builder.Append(" (");
        builder.Append(sinceAddition.Humanize(3, minUnit: TimeUnit.Minute, culture: new("cs-CS")));
        if (i != emotes.Count - 1)
          builder.Append("), ");
        else
          builder.Append(").");
      }

      Bot.WriteMessage(builder.ToString(), message.Channel);
      TimesUsedSinceRestart++;
      LastUsed = DateTime.Now;
    }
  }
}
