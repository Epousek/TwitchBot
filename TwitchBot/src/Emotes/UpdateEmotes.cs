using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using TwitchBot.Connections;
using TwitchBot.Models;

namespace TwitchBot.Emotes
{
  internal static class UpdateEmotes
  {
    public static async Task StartUpdatingEmotes(List<string> channels)
    {
      await Task.Run(async () =>
      {
        while (true)
        {
          foreach(var channel in channels)
            await CompareEmotes(channel).ConfigureAwait(false);
          Thread.Sleep(TimeSpan.FromMinutes(1));
        }
      }).ConfigureAwait(false);
    }

    private static async Task CompareEmotes(string channel)
    {
      var fromDb = await GetEmotes.EmotesFromDb(channel).ConfigureAwait(false);
      var fromApi = new List<EmoteModel>();
      var notInDb = new List<EmoteModel>();
      var notActiveAnymore = new List<EmoteModel>();
      var activeAgain = new List<EmoteModel>();

      try
      {
        fromApi.AddRange(await GetEmotes.BttvApiAsync(channel).ConfigureAwait(false));
      } 
      catch (Exception e)
      {
        Log.Error(e, "No emotes from bttv api: {message}", e.Message);
        return;
      }
      try
      {
        fromApi.AddRange(await GetEmotes.FfzApiAsync(channel).ConfigureAwait(false));
      } 
      catch (Exception e)
      {
        Log.Error(e, "No emotes from ffz api: {message}", e.Message);
        return;
      }
      try
      {
        fromApi.AddRange(await GetEmotes.SeventvApiAsync(channel).ConfigureAwait(false));
      } 
      catch (Exception e)
      {
        Log.Error(e, "No emotes from 7tv api: {message}", e.Message);
        return;
      }

      foreach (var emote in fromApi.Where(emote 
                 => !fromDb.Any(x 
                   => x.Name == emote.Name && x.Service == emote.Service)))
      {
        emote.Added = DateTime.Now;
        notInDb.Add(emote);
      }

      foreach(var emote in fromDb.Where(x => x.IsActive))
      {
        if (fromApi.Any(x => x.Name == emote.Name && x.Service == emote.Service)) 
          continue;
        emote.Removed = DateTime.Now;
        emote.IsActive = false;
        notActiveAnymore.Add(emote);
      }

      foreach (var emote in fromApi.Where(emote 
                 => fromDb.Any(x 
                   => x.Name == emote.Name && x.Service == emote.Service && !x.IsActive)))
      {
        emote.Added = DateTime.Now;
        emote.IsActive = true;
        activeAgain.Add(emote);
      }

      if (notInDb.Count > 0)
        await DatabaseConnections.WriteEmotes(channel, notInDb).ConfigureAwait(false);
      if (notActiveAnymore.Count > 0)
        await DatabaseConnections.UpdateEmotes(channel, notActiveAnymore).ConfigureAwait(false);
      if (activeAgain.Count > 0)
        await DatabaseConnections.UpdateEmotes(channel, activeAgain).ConfigureAwait(false);
    }
  }
}
