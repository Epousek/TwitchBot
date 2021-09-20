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
      await Task.Run(async () =>
      {
        while (true)
        {
          await Task.Run(RefreshAccessToken).ConfigureAwait(false);
          Thread.Sleep(TimeSpan.FromHours(1));
        }
      }).ConfigureAwait(false);
    }

    private async static Task RefreshAccessToken()
    {
      Console.WriteLine("trying to refresh access token");

      RestClient client = new("https://id.twitch.tv/oauth2/token");
      RestRequest request = new() { Method = Method.POST };

      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddHeader("Accept", "application/json");
      request.AddParameter("grant_type", "refresh_token");
      request.AddParameter("refresh_token", Config.Credentials.RefreshToken);
      request.AddParameter("client_id", Config.Credentials.ClientID);
      request.AddParameter("client_secret", Config.Credentials.Secret);

      var response = await client.ExecuteAsync(request).ConfigureAwait(false);
      if (response.IsSuccessful)
      {
        Console.WriteLine("successfully refreshed access token");
        JObject jsonResponse = JObject.Parse(response.Content);
        AuthResponse tokens = jsonResponse.ToObject<AuthResponse>();
        Config.SetTokens(tokens);
      }
      else
      {
        Console.WriteLine("Failed to refresh token - NEED NEW ACCESS TOKEN"); //todo: log to file 
        Config.SetConfig();
      }
    }
  }
}
