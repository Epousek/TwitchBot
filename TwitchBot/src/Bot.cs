using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using TwitchBot.Commands;
using TwitchBot.Commands.Status;
using TwitchBot.Connections;
using TwitchBot.Models;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchBot
{
  internal class Bot
  {
    public static CommandGetter CmdGetter;
    public static string Prefix;
    public bool Reconnecting;
    public static TwitchClient _client;
    private static bool _firstInit = true;
    private static DateTime _lastMessageSent;
    private static string _lastMessageChannel;

    public Bot(List<string> channelsToConnectTo)
    {
      if (_firstInit)
      {
        Log.Information("First Bot.cs initialization");
        Prefix = Debugger.IsAttached ? "$$" : "$";
        CmdGetter = new CommandGetter();
        _lastMessageChannel = string.Empty;
        _lastMessageSent = DateTime.Now;
        _firstInit = false;
      }
      else
      {
        _client?.Disconnect();
        Thread.Sleep(100);
      }

      var creds = new ConnectionCredentials(SecretsConfig.Credentials.Username, SecretsConfig.Credentials.AccessToken);
      var clientOptions = new ClientOptions
      {
        MessagesAllowedInPeriod = 750,
        ThrottlingPeriod = TimeSpan.FromSeconds(30)
      };
      var wsClient = new WebSocketClient(clientOptions);
      _client = new TwitchClient(wsClient);
      _client.Initialize(creds, channelsToConnectTo);

      //client.OnLog += Client_OnLog;
      //client.OnConnected += Client_OnConnected;
      _client.OnMessageReceived += Client_OnMessageReceived;
      _client.OnJoinedChannel += Client_OnJoinedChannel;
      _client.OnConnected += Client_OnConnected;
      _client.OnError += Client_OnError;
      _client.OnDisconnected += Client_OnDisconnected;

      _client.Connect();
    }

    public static void WriteMessage(string message, string channel)
    {
      if((DateTime.Now - _lastMessageSent) < TimeSpan.FromMilliseconds(1000) && _lastMessageChannel == channel) //not working
        Thread.Sleep(1000); //TODO: this is not working :/
      _client.SendMessage(channel, message);
      _lastMessageChannel = channel;
      _lastMessageSent = DateTime.Now;
    }

    // public static async Task StartReconnecting()
    // {
    //   await Task.Run(() =>
    //   {
    //     while (true)
    //     {
    //       Thread.Sleep(TimeSpan.FromDays(1));
    //       _client.Reconnect();
    //     }
    //   }).ConfigureAwait(false);
    // }

    private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
    {
      Log.Information("{username} connected.", e.BotUsername);
      // if (_client.JoinedChannels.Count != 0) 
      //   return;
      // foreach (var channel in _channels)
      // {
      //   _client.JoinChannel(channel);
      // }
    }

    private void Client_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
    {
      Log.Information("Joined {channel}.", e.Channel);
    }

    private async void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
      var message = new ChatMessageModel
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
      await GetSetStatus.CheckStatus(message).ConfigureAwait(false);

      if (e.ChatMessage.Message.StartsWith(Prefix))
        await CmdGetter.CheckIfCommandAsync(message).ConfigureAwait(false);
    }

    private async void Client_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
    {
      if (Reconnecting)
      {
        Log.Information("Intentionally disconnected.");
        Reconnecting = false;
        return;
      }

      Log.Warning("Twitch client disconnected.");
      var timeToWait = 500;

      while (!_client.IsConnected)
      {
        try
        {
          _client.Connect();
          if (_client.JoinedChannels.Count != 0) 
            continue;
          Thread.Sleep(500);
          foreach (var channel in await DatabaseConnections.GetConnectedChannels().ConfigureAwait(false))
          {
            _client.JoinChannel(channel);
          }

          break;
        }
        catch (Exception ex)
        {
          Log.Error("Couldn't reconnect: {ex}", ex);
        }

        if (timeToWait < 60000)
        {
          Thread.Sleep(timeToWait);
          timeToWait = timeToWait * 2;
        }
        else
        {
          Log.Fatal("Couldn't reconnect even after trying a lot :/");
          break;
        }
      }

      // while (true)
      // {
      //   try
      //   {
      //     if (!_client.IsConnected)
      //       _client.Connect();
      //     break;
      //   }
      //   catch (Exception ex)
      //   {
      //     Log.Debug("Can't reconnect: {error}", ex.Message);
      //   }
      //   await Task.Delay(5000);
      //   Log.Debug("{bool}", _client.IsConnected);
      // }
      // _reconnect = false;
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