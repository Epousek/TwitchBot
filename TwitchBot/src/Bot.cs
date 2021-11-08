using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchBot.src.Connections;
using TwitchBot.src.Models;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchBot.src
{
  class Bot
  {
    static TwitchClient client;

    public Bot(List<string> channelsToConnectTo)
    {
      ConnectionCredentials creds = new(SecretsConfig.Credentials.Username, SecretsConfig.Credentials.AccessToken);
      var clientOptions = new ClientOptions
      {
        MessagesAllowedInPeriod = 750,
        ThrottlingPeriod = TimeSpan.FromSeconds(30)
      };
      WebSocketClient wsClient = new(clientOptions);
      client = new(wsClient);
      client.Initialize(creds, channelsToConnectTo);

      //client.OnLog += Client_OnLog;
      //client.OnConnected += Client_OnConnected;
      client.OnMessageReceived += Client_OnMessageReceived;
      client.OnJoinedChannel += Client_OnJoinedChannel;

      client.Connect();
    }

    //private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e) 
    //{
    //  Log.Information("Connected to {channel}.", e.AutoJoinChannel);
    //}

    private void Client_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
    {
      Log.Information("Joined {channel}.", e.Channel);
    }

    private async void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
      ChatMessageModel msg = new() {
        Channel = e.ChatMessage.Channel,
        Username = e.ChatMessage.Username,
        Message = e.ChatMessage.Message,
        TimeStamp = DateTime.Now
      };
      Log.Debug("{channel} - {name}: {message}", msg.Channel, msg.Username, msg.Message);

      await DatabaseConnections.WriteMessage(msg).ConfigureAwait(false);
    }

    //private void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
    //{
    //  Console.WriteLine(e.DateTime.ToString() + " " + e.Data);
    //}
  }
}