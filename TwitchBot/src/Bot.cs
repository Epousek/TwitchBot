using System;
using System.Collections.Generic;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchBot.src
{
  class Bot
  {
    TwitchClient client;

    public Bot(List<string> channelsToConnectTo)
    {
      ConnectionCredentials creds = new(Config.Credentials.Username, Config.Credentials.AccessToken);
      var clientOptions = new ClientOptions
      {
        MessagesAllowedInPeriod = 750,
        ThrottlingPeriod = TimeSpan.FromSeconds(30)
      };
      WebSocketClient wsClient = new(clientOptions);
      client = new(wsClient);
      client.Initialize(creds, channelsToConnectTo);

      //client.OnLog += Client_OnLog;
      client.OnConnected += Client_OnConnected;
      client.OnMessageReceived += Client_OnMessageReceived;
      client.OnJoinedChannel += Client_OnJoinedChannel;

      client.Connect();
    }

    private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e) 
    {
      Console.WriteLine("connected to " + e.AutoJoinChannel);
    }

    private void Client_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e) 
    {
      Console.WriteLine(e.Channel);
    }

    private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
      Console.WriteLine($"{e.ChatMessage.Channel} - {e.ChatMessage.DisplayName}: {e.ChatMessage.Message}");
    }

    //private void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
    //{
    //  Console.WriteLine(e.DateTime.ToString() + " " + e.Data);
    //}
  }
}