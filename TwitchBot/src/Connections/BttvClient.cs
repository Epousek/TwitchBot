using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using TwitchBot.Models;

namespace TwitchBot.Connections
{
  public class BttvClient
  {
    private readonly RestClient _client;

    public BttvClient()
    {
      _client = new RestClient("https://api.betterttv.net/3/cached/users/twitch/");
    }

    public async Task<List<EmoteModel>> GetEmotesAsync(int id)
    {
      var emotes = new List<EmoteModel>();
      var request = new RestRequest(id.ToString());
      var response = await _client.GetAsync(request);
      
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
      var contentObject = JObject.Parse(content!);

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
  }
}