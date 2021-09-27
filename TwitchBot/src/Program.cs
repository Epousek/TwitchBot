using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TwitchBot.src.Connections;

namespace TwitchBot.src
{
  class Program
  {
    static List<string> channelsToConnectTo;

    static async Task Main(string[] args)
    {
      Config.SetConfig();
      Console.WriteLine(Config.Credentials.Username);

      Thread authThread = new (() => Authentication.StartRefreshingTokensAsync());
      authThread.Start();

      await Task.Run(SetChannelsToConnectToAsync);
      Bot bot = new(channelsToConnectTo);
      Console.ReadLine();
    }

    static async Task SetChannelsToConnectToAsync()
      => channelsToConnectTo = await DatabaseConnections.GetConnectedChannels();
  }
}
