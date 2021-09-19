using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace TwitchBot.src
{
  static class Authentication
  {
    public static async Task StartRefreshingTokens()
    {
      await Task.Run(() =>
      {
        while (true)
        {
          Task.Run(RefreshAccessToken);
          Thread.Sleep(TimeSpan.FromHours(1));
        }
      });
    }

    private async static Task RefreshAccessToken()
    {
      var client = new RestClient("https://id.twitch.tv/oauth2/token");
      RestRequest request = new RestRequest() { Method = Method.POST };

      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddHeader("Accept", "application/json");
      request.AddParameter("grant_type", "refresh_token");
      request.AddParameter("refresh_token", Config.Credentials.RefreshToken);
      request.AddParameter("client_id", Config.Credentials.ClientID);
      request.AddParameter("client_secret", Config.Credentials.Secret);

      var response = await client.ExecuteAsync(request);
      JObject jsonResponse = JObject.Parse(response.Content);
      AuthResponse tokens = jsonResponse.ToObject<AuthResponse>();
      Config.SetTokens(tokens);
    }
  }
}
