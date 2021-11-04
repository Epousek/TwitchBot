using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;
using Serilog;
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
      Log.Information("Trying to validate app access token");
      RestClient client = new("https://id.twitch.tv/oauth2/validate");
      RestRequest request = new();

      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddHeader("Accept", "application/json");
      request.AddHeader("Authorization", "Bearer " + SecretsConfig.Credentials.AccessToken);

      var response = await client.ExecuteAsync(request).ConfigureAwait(false);
      if (response.IsSuccessful)
      {
        JObject responseJson = JObject.Parse(response.Content);
        TokenValidationResponse validationResponse = responseJson.ToObject<TokenValidationResponse>();
        if(validationResponse.ExpiresIn < 5400)
        {
          Log.Information("Token is about to expire, refreshing.");
          await RefreshTokens();
        }
        else
        {
          Log.Information("Token expires in about {expiresIn} hours", validationResponse.ExpiresIn / 3600);
        }
      }
      else
      {
        Log.Error("Couldn't validate: {statusDescription}", response.StatusDescription);
      }
    }

    private static async Task RefreshTokens()
    {
      Log.Information("Trying to refresh token.");

      RestClient client = new ("https://id.twitch.tv/oauth2/token");
      RestRequest request = new () { Method = Method.POST };

      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddHeader("Accept", "application/json");
      request.AddParameter("client_id", SecretsConfig.Credentials.ClientID);
      request.AddParameter("client_secret", SecretsConfig.Credentials.Secret);
      request.AddParameter("grant_type", "refresh_token");
      request.AddParameter("refresh_token", SecretsConfig.Credentials.RefreshToken);

      var response = await client.ExecuteAsync(request).ConfigureAwait(false);
      if (response.IsSuccessful)
      {
        Log.Information("Refreshed tokens.");

        JObject responseJson = JObject.Parse(response.Content);
        AppAccessToken newToken = responseJson.ToObject<AppAccessToken>();
        await SecretsConfig.SetToken(newToken);
      }
      else
      {
        Log.Error(response.ErrorException, "Failed to refresh token: {statusDescription}.", response.StatusDescription);
      }
    }
  }
}
