using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchBot.src.Models;
using TwitchBot.src.Emotes;

namespace TwitchBot.src.Emotes
{
  static class UpdateEmotes
  {
    public static async Task CompareEmotes(string channel)
    {
      List<EmoteModel> fromDB = await GetEmotes.EmotesFromDB(channel).ConfigureAwait(false);
      List<EmoteModel> fromAPI = new();
      List<EmoteModel> notInDB = new();
      List<EmoteModel> notActiveAnymore = new();

      foreach (EmoteModel emote in await GetEmotes.BttvAPIAsync(channel).ConfigureAwait(false))
        fromAPI.Add(emote);
      foreach (EmoteModel emote in await GetEmotes.FfzAPIAsync(channel).ConfigureAwait(false))
        fromAPI.Add(emote);
      foreach (EmoteModel emote in await GetEmotes.SeventvAPIAsync(channel).ConfigureAwait(false))
        fromAPI.Add(emote);

      foreach(EmoteModel emote in fromAPI)
      {
        if (!fromDB.Any(x => x.Name == emote.Name && x.Service == emote.Service))
        {
          emote.Added = DateTime.Now;
          notInDB.Add(emote);
        }
      }

      foreach(EmoteModel emote in fromDB.Where(x => x.IsActive))
      {
        if(!fromAPI.Any(x => x.Name == emote.Name && x.Service == emote.Service))
        {
          emote.Removed = DateTime.Now;
          notActiveAnymore.Add(emote);
        }
      }
    }
  }
}
