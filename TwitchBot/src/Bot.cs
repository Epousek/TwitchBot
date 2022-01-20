using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
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
    public static CommandGetter cg;
    private static TwitchClient client;
    private List<string> channels;
    private bool reconnect;

    public Bot(List<string> channelsToConnectTo)
    {
      cg = new CommandGetter();
      channels = channelsToConnectTo;

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
      client.OnConnected += Client_OnConnected;
      client.OnError += Client_OnError;
      client.OnDisconnected += Client_OnDisconnected;

      client.Connect();
    }

    public static void WriteMessage(string message, string channel)
      => client.SendMessage(channel, message);

    private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
    {
      Log.Information("{username} connected.", e.BotUsername);
      if (client.JoinedChannels.Count == 0)
      {
        foreach (string channel in channels)
        {
          client.JoinChannel(channel);
        }
      }
    }

    private void Client_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
    {
      Log.Information("Joined {channel}.", e.Channel);
    }

    private async void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
      ChatMessageModel message = new()
      {
        Channel = e.ChatMessage.Channel,
        Username = e.ChatMessage.Username,
        Message = e.ChatMessage.Message,
        TimeStamp = DateTime.Now
      };

      if (message.Message.Contains("󠀀"))
        message.Message = message.Message.Replace("󠀀", "");

      Log.Debug("{channel} - {name}: {message}", message.Channel, message.Username, message.Message);
      await DatabaseConnections.WriteMessage(message).ConfigureAwait(false);

      await Remind.CheckForReminder(message).ConfigureAwait(false);

      if (e.ChatMessage.Message.StartsWith("$"))
        await cg.CheckIfCommandAsync(message).ConfigureAwait(false);
    }

    private async void Client_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
    {
      if (!reconnect)
      {
        reconnect = true;

        Log.Warning("Twitch client disconnected.");

        while (true)
        {
          try
          {
            if (!client.IsConnected)
              client.Connect();
            break;
          }
          catch (Exception ex)
          {
            Log.Debug("Can't reconnect: {error}", ex.Message);
          }
          await Task.Delay(5000);
          Log.Debug("{bool}", client.IsConnected);
        }
        reconnect = false;
      }
    }

    private void Client_OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
    {
      Log.Error("TwitchLib error: {0}: {1}", e.Exception, e.Exception.Message);
    }

    //private void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
    //{
    //  Console.WriteLine(e.DateTime.ToString() + " " + e.Data);
    //}
  }
}