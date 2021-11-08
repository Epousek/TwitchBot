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
        RestClient client = new("https://api.betterttv.net/3/cached/users/twitch/" + id.ToString());
        RestRequest request = new();
        
        RestResponse response = (RestResponse)await client.ExecuteAsync(request).ConfigureAwait(false);

        if (!response.IsSuccessful)
        {
          Log.Error(response.ErrorException, "Couldn't get emotes from BTTV API: {statusDescription}", response.StatusDescription);
          return null;
        }

        string content = response.Content;
        JObject contentObject = JObject.Parse(content);

        foreach (JObject item in JArray.Parse(contentObject["channelEmotes"].ToString()))
        {
          EmoteModel emote = item.ToObject<EmoteModel>();
          emote.Service = "bttv";
          emotes.Add(emote);
        }
        foreach (JObject item in JArray.Parse(contentObject["sharedEmotes"].ToString()))
        {
          EmoteModel emote = item.ToObject<EmoteModel>();
          emote.Service = "bttv";
          emotes.Add(emote);
        }

        return emotes;
      }
    }

    public static async Task<List<EmoteModel>> FfzAPIAsync(string channel)
    {
      List<EmoteModel> emotes = new();

      RestClient client = new("https://api.frankerfacez.com/v1/room/" + channel.ToLower());
      RestRequest request = new();

      RestResponse response = (RestResponse)await client.ExecuteAsync(request).ConfigureAwait(false);

      if (!response.IsSuccessful)
      {
        Log.Error(response.ErrorException, "Couldn't get emotes from BTTV API: {statusDescription}", response.StatusDescription);
        return null;
      }

      string content = response.Content;
      JObject room = JObject.Parse(content);
      string setID = room["room"]["set"].ToString();
      JObject set = (JObject)room["sets"][setID];

      foreach (JObject item in JArray.Parse(set["emoticons"].ToString()))
      {
        EmoteModel emote = item.ToObject<EmoteModel>();
        emote.Service = "ffz";
        emotes.Add(emote);
      }

      return emotes;
    }

    public static async Task<List<EmoteModel>> SeventvAPIAsync(string channel)
    {
      List<EmoteModel> emotes = new();

      RestClient client = new($"https://api.7tv.app/v2/users/{channel}/emotes");
      RestRequest request = new();

      RestResponse response = (RestResponse)await client.ExecuteAsync(request).ConfigureAwait(false);
      if (!response.IsSuccessful)
      {
        Log.Error(response.ErrorException, "Couldn't get emotes from BTTV API: {statusDescription}", response.StatusDescription);
        return null;
      }

      string content = response.Content;
      foreach (JObject item in JArray.Parse(content))
      {
        EmoteModel emote = item.ToObject<EmoteModel>();
        emote.Service = "7tv";
        emotes.Add(emote);
      }

      return emotes;
    }

    public static async Task<List<EmoteModel>> EmotesFromDB(string channel)
      => await DatabaseConnections.GetEmotes(channel).ConfigureAwait(false);
  }
}
