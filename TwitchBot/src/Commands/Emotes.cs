using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Connections;
using TwitchBot.src.Models;
using TwitchBot.src.Enums;
using Humanizer;
using Humanizer.Localisation;

namespace TwitchBot.src.Commands
{
  class Emotes : ICommand
  {
    public string Name { get; } = nameof(Emotes);
    public string About { get; } = "Vypíše naposledy přidané emoty";
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
      if (DateTime.Now - LastUsed < Cooldown)
        return;

      TimeSpan sinceAddition;
      List<EmoteModel> emotes = await DatabaseConnections.GetLastAddedEmotes(message.Channel);
      StringBuilder builder = new("@");
      builder.Append(message.Username);
      builder.Append(" Naposledy přidané emoty: ");

      foreach (EmoteModel emote in emotes)
      {
        sinceAddition = (TimeSpan)(DateTime.Now - emote.Added);
        if (builder.Length < 290)
        {
          builder.Append(emote.Name);
          builder.Append(" (");
          builder.Append(sinceAddition.Humanize(3, minUnit: TimeUnit.Minute));
          builder.Append("), ");
        }
        else
        {
          builder.Append(emote.Name);
          builder.Append(" (");
          builder.Append(sinceAddition.Humanize(3, minUnit: TimeUnit.Minute));
          builder.Append(").");
          break;
        }
      }

      Bot.WriteMessage(builder.ToString(), message.Channel);
      TimesUsedSinceRestart++;
      LastUsed = DateTime.Now;
    }
  }
}
