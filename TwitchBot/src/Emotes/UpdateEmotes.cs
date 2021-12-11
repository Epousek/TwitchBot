using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwitchBot.src.Models;
using TwitchBot.src.Connections;
using Serilog;

namespace TwitchBot.src.Emotes
{
  static class UpdateEmotes
  {
    public static async Task StartUpdatingEmotes(List<string> channels)
    {
      await Task.Run(async () =>
      {
        while (true)
        {
          foreach(string channel in channels)
            await CompareEmotes(channel).ConfigureAwait(false);
          Thread.Sleep(TimeSpan.FromMinutes(1));
        }
      }).ConfigureAwait(false);
    }

    private static async Task CompareEmotes(string channel)
    {
      List<EmoteModel> fromDB = await GetEmotes.EmotesFromDB(channel).ConfigureAwait(false);
      List<EmoteModel> fromAPI = new();
      List<EmoteModel> notInDB = new();
      List<EmoteModel> notActiveAnymore = new();
      List<EmoteModel> activeAgain = new();

      try
      {
        foreach (EmoteModel emote in await GetEmotes.BttvAPIAsync(channel).ConfigureAwait(false))
          fromAPI.Add(emote);
      } catch (Exception e)
      {
        Log.Error(e, "No emotes from bttv api: ", e.Message);
        return;
      }
      try
      {
        foreach (EmoteModel emote in await GetEmotes.FfzAPIAsync(channel).ConfigureAwait(false))
          fromAPI.Add(emote);
      } catch (Exception e)
      {
        Log.Error(e, "No emotes from ffz api: ", e.Message);
        return;
      }
      try
      {
        foreach (EmoteModel emote in await GetEmotes.SeventvAPIAsync(channel).ConfigureAwait(false))
          fromAPI.Add(emote);
      } catch (Exception e)
      {
        Log.Error(e, "No emotes from 7tv api: ", e.Message);
        return;
      }

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
          emote.IsActive = false;
          notActiveAnymore.Add(emote);
        }
      }

      foreach(EmoteModel emote in fromAPI)
      {
        if(fromDB.Any(x => x.Name == emote.Name && x.Service == emote.Service && !x.IsActive))
        {
          emote.Added = DateTime.Now;
          emote.IsActive = true;
          activeAgain.Add(emote);
        }
      }

      if (notInDB.Count > 0)
        await DatabaseConnections.WriteEmotes(channel, notInDB).ConfigureAwait(false);
      if (notActiveAnymore.Count > 0)
        await DatabaseConnections.UpdateEmotes(channel, notActiveAnymore).ConfigureAwait(false);
      if (activeAgain.Count > 0)
        await DatabaseConnections.UpdateEmotes(channel, activeAgain).ConfigureAwait(false);
    }
  }
}
