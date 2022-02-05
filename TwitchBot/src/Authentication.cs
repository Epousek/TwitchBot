using System;
using System.Threading;
using System.Threading.Tasks;
using MySqlX.XDevAPI;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using TwitchBot.Connections;
using TwitchBot.Models;

namespace TwitchBot
{
  internal static class Authentication
  {
    private static TwitchApiClient _client;

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

    private static async Task ValidateAccessToken()
    {
      _client ??= new TwitchApiClient();
      await _client.ValidateAccessToken().ConfigureAwait(false);
    }
  }
}
