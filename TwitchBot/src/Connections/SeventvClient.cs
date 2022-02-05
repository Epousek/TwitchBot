using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using TwitchBot.Models;

namespace TwitchBot.Connections
{
  public class SeventvClient
  {
    private readonly RestClient _client;

    public SeventvClient()
    {
      _client = new RestClient("https://api.7tv.app/v2/users/");
    }

    public async Task<List<EmoteModel>> GetEmotesAsync(string channel)
    {
      var emotes = new List<EmoteModel>();
      var request = new RestRequest($"{channel.ToLower()}/emotes");
      var response = await _client.GetAsync(request);

      
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
  }
}