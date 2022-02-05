using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using TwitchBot.Models;

namespace TwitchBot.Connections
{
  public class FfzClient
  {
    private readonly RestClient _client;

    public FfzClient()
    {
      _client = new RestClient("https://api.frankerfacez.com/v1/room/");
    }

    public async Task<List<EmoteModel>> GetEmotesAsync(string channel)
    {
      var emotes = new List<EmoteModel>();
      var request = new RestRequest(channel.ToLower());
      var response = await _client.GetAsync(request);

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
      if (content == null) 
        return emotes;
      
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
  }
}