using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchBot.src.Models;
using TwitchBot.src.Connections;
using RestSharp;
using Serilog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitchBot.src.Emotes
{
  static class GetEmotes
  {
    public static async Task<List<EmoteModel>> BttvAPIAsync(string channel)
    {
      List<EmoteModel> emotes = new ();
      int id = await DatabaseConnections.GetConnectedChannelID(channel).ConfigureAwait(false);

      if(id == -1)
      {
        Log.Error("Didn't get ID for {channel}", channel);
        return null;
      }
      else
      {
        string url = "https://api.betterttv.net/3/cached/users/twitch/" + id.ToString();
        RestClient client = new(url);
        RestRequest request = new();

        RestResponse response = (RestResponse)await client.ExecuteAsync(request).ConfigureAwait(false);
        string content = response.Content;
        JObject contentObject = JObject.Parse(content);

        JArray channelEmotes = JArray.Parse(contentObject["channelEmotes"].ToString());
        JArray sharedEmotes = JArray.Parse(contentObject["sharedEmotes"].ToString());

        foreach (JObject item in channelEmotes)
        {
          EmoteModel emote = item.ToObject<EmoteModel>();
          emote.Service = "bttv";
          emotes.Add(emote);
        }
        foreach (JObject item in sharedEmotes)
        {
          EmoteModel emote = item.ToObject<EmoteModel>();
          emote.Service = "bttv";
          emotes.Add(emote);
        }

        return emotes;
      }
    }
    public static async Task<List<EmoteModel>> BttvDBAsync()
    {
      List<EmoteModel> emotes = new();



      return emotes;
    }
    public static async Task<List<EmoteModel>> FfzAPIAsync()
    {
      List<EmoteModel> emotes = new();



      return emotes;
    }
    public static async Task<List<EmoteModel>> FfzDBAsync()
    {
      List<EmoteModel> emotes = new();



      return emotes;
    }
    public static async Task<List<EmoteModel>> SeventvAPIAsync()
    {
      List<EmoteModel> emotes = new();



      return emotes;
    }
    public static async Task<List<EmoteModel>> SeventvDBAsync()
    {
      List<EmoteModel> emotes = new();



      return emotes;
    }
  }
}
