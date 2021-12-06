using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchBot.src.Commands;
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
    static CommandGetter cg;

    public Bot(List<string> channelsToConnectTo)
    {
      cg = new();

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

    public static void WriteMessage(string message, string channel)
      => client.SendMessage(channel, message);

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

      ChatMessageModel message = new() {
        Channel = e.ChatMessage.Channel,
        Username = e.ChatMessage.Username,
        Message = e.ChatMessage.Message,
        TimeStamp = DateTime.Now
      };
      Log.Debug("{channel} - {name}: {message}", message.Channel, message.Username, message.Message);
      
      if (e.ChatMessage.Message.StartsWith("$"))
        await cg.CheckIfCommandAsync(message);
      
      await DatabaseConnections.WriteMessage(message).ConfigureAwait(false);
    }

    //private void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
    //{
    //  Console.WriteLine(e.DateTime.ToString() + " " + e.Data);
    //}
  }
}