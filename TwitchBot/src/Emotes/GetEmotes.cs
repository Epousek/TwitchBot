using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using TwitchBot.Connections;
using TwitchBot.Models;

namespace TwitchBot.Emotes
{
  public static class GetEmotes
  {
    private static BttvClient _bttvClient;
    private static FfzClient _ffzClient;
    private static SeventvClient _seventvClient;

    public static async Task<List<EmoteModel>> BttvApiAsync(string channel)
    {
      _bttvClient ??= new BttvClient();
      
      var emotes = new List<EmoteModel>();
      var id = await DatabaseConnections.GetConnectedChannelId(channel).ConfigureAwait(false);

      if (id != -1) 
        return await _bttvClient.GetEmotesAsync(id).ConfigureAwait(false);
      
      Log.Error("Didn't get ID for {channel}", channel);
      return null;
    }

    public static async Task<List<EmoteModel>> FfzApiAsync(string channel)
    {
      _ffzClient ??= new FfzClient();
      return await _ffzClient.GetEmotesAsync(channel).ConfigureAwait(false);
    }

    public static async Task<List<EmoteModel>> SeventvApiAsync(string channel)
    {
      _seventvClient ??= new SeventvClient();
      return await _seventvClient.GetEmotesAsync(channel);
    }

    public static async Task<List<EmoteModel>> EmotesFromDb(string channel)
      => await DatabaseConnections.GetEmotes(channel).ConfigureAwait(false);
  }
}
