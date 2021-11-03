using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;
using TwitchBot.src.Models;

namespace TwitchBot.src
{
  static class Authentication
  {
    public static async Task StartValidatingTokenAsync()
    {
      await Task.Run(async () =>
      {
        while (true)
        {
          await Task.Run(ValidateAccessToken).ConfigureAwait(false);
          Thread.Sleep(TimeSpan.FromHours(1));
        }
      }).ConfigureAwait(false);
    }

    private async static Task ValidateAccessToken()
    {
      //log
      RestClient client = new("https://id.twitch.tv/oauth2/validate");
      RestRequest request = new();

      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddHeader("Accept", "application/json");
      request.AddHeader("Authorization", "Bearer " + Config.Credentials.AccessToken);

      var response = await client.ExecuteAsync(request).ConfigureAwait(false);
      if (response.IsSuccessful)
      {
        JObject responseJson = JObject.Parse(response.Content);
        TokenValidationResponse validationResponse = responseJson.ToObject<TokenValidationResponse>();
        if(validationResponse.ExpiresIn < 36000)
        {
          Console.WriteLine("token about to expire, getting new one");
          await GetNewToken();
          //log
        }
        else
        {
          Console.WriteLine("token still valid, expires in: {0} seconds", validationResponse.ExpiresIn);
          //log
        }
      }
      else
      {
        if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
          Console.WriteLine("invalid token, getting new one");
          await GetNewToken();
          //log
        }
        //log
      }
    }

    private static async Task GetNewToken()
    {
      RestClient client = new ("https://id.twitch.tv/oauth2/token");
      RestRequest request = new () { Method = Method.POST };

      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddHeader("Accept", "application/json");
      request.AddParameter("client_id", Config.Credentials.ClientID);
      request.AddParameter("client_secret", Config.Credentials.Secret);
      request.AddParameter("grant_type", "client_credentials");

      var response = await client.ExecuteAsync(request).ConfigureAwait(false);
      if (response.IsSuccessful)
      {
        Console.WriteLine("got new token");

        JObject responseJson = JObject.Parse(response.Content);
        AppAccessToken newToken = responseJson.ToObject<AppAccessToken>();
        Config.SetToken(newToken);
        //log
      }
      else
      {
        Console.WriteLine("failed to get new token");
        //log
      }
    }
  }
}
