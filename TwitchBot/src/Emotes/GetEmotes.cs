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
    public static async Task<List<EmoteModel>> BttvApiAsync(string channel)
    {
      var emotes = new List<EmoteModel>();
      var id = await DatabaseConnections.GetConnectedChannelId(channel).ConfigureAwait(false);

      if(id == -1)
      {
        Log.Error("Didn't get ID for {channel}", channel);
        return null;
      }

      var client = new RestClient("https://api.betterttv.net/3/cached/users/twitch/" + id);
      var request = new RestRequest();

      var response = (RestResponse)await client.ExecuteAsync(request).ConfigureAwait(false);

      if (!response.IsSuccessful)
      {
        if(response.ErrorException == null)
        {
          Log.Error("BTTV API currently unavailable.");
          return null;
        }
        Log.Error(response.ErrorException, "Couldn't get emotes from BTTV API: {statusDescription}", response.ErrorMessage);
        return null;
      }

      var content = response.Content;
      var contentObject = JObject.Parse(content);

      foreach (var jToken in JArray.Parse(contentObject["channelEmotes"]?.ToString() ?? string.Empty))
      {
        if (jToken == null)
          continue;
        var item = (JObject)jToken;
        var emote = item.ToObject<EmoteModel>();
        if (emote == null) 
          continue;
        emote.Service = "bttv";
        emotes.Add(emote);
      }
      foreach (var jToken in JArray.Parse(contentObject["sharedEmotes"]?.ToString() ?? string.Empty))
      {
        if (jToken == null)
          continue;
        var item = (JObject)jToken;
        var emote = item.ToObject<EmoteModel>();
        if (emote == null) 
          continue;
        emote.Service = "bttv";
        emotes.Add(emote);
      }

      return emotes;
    }

    public static async Task<List<EmoteModel>> FfzApiAsync(string channel)
    {
      var emotes = new List<EmoteModel>();

      var client = new RestClient("https://api.frankerfacez.com/v1/room/" + channel.ToLower());
      var request = new RestRequest();

      var response = (RestResponse)await client.ExecuteAsync(request).ConfigureAwait(false);

      if (!response.IsSuccessful)
      {
        if (response.ErrorException == null)
        {
          Log.Error("FFZ API currently unavailable.");
          return null;
        }
        Log.Error(response.ErrorException, "Couldn't get emotes from BTTV API: {statusDescription}", response.ErrorMessage);
        return null;
      }

      var content = response.Content;
      var room = JObject.Parse(content);
      var setId = room["room"]["set"].ToString();
      var set = (JObject)room["sets"][setId];

      foreach (var jToken in JArray.Parse(set?["emoticons"]?.ToString() ?? string.Empty))
      {
        if(jToken == null)
          continue;
        var item = (JObject)jToken;
        var emote = item.ToObject<EmoteModel>();
        if (emote == null)
          continue;
        emote.Service = "ffz";
        emotes.Add(emote);
      }

      return emotes;
    }

    public static async Task<List<EmoteModel>> SeventvApiAsync(string channel)
    {
      var emotes = new List<EmoteModel>();

      var client = new RestClient($"https://api.7tv.app/v2/users/{channel}/emotes");
      var request = new RestRequest();

      var response = (RestResponse)await client.ExecuteAsync(request).ConfigureAwait(false);

      if (!response.IsSuccessful)
      {
        if (response.ErrorException == null)
        {
          Log.Error("7tv API currently unavailable.");
          return null;
        }
        Log.Error(response.ErrorException, "Couldn't get emotes from BTTV API: {statusDescription}", response.ErrorMessage);
        return null;
      }

      var content = response.Content;
      foreach (var jToken in JArray.Parse(content))
      {
        if (jToken == null)
          continue;
        var item = (JObject)jToken;
        var emote = item.ToObject<EmoteModel>();
        if(emote == null)
          continue;
        emote.Service = "7tv";
        emotes.Add(emote);
      }

      return emotes;
    }

    public static async Task<List<EmoteModel>> EmotesFromDb(string channel)
      => await DatabaseConnections.GetEmotes(channel).ConfigureAwait(false);
  }
}
