using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.MariaDB.Extensions;
using TwitchBot.Commands;
using TwitchBot.Connections;

namespace TwitchBot
{
  internal static class Program
  {
    private static List<string> _channelsToConnectTo;
    public static Bot bot;

    private static async Task Main()
    {
      BotInfo.RunningSince = DateTime.Now;
      await SecretsConfig.SetConfig().ConfigureAwait(false);

      var builder = new ConfigurationBuilder();
      BuildSettingsConfing(builder);

      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .ReadFrom.Configuration(builder.Build())
        .Enrich.FromLogContext()
        .WriteTo.MariaDB(
        SecretsConfig.Credentials.ConnectionString,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        autoCreateTable: true,
        tableName: "Logs"
        )
        .CreateLogger();

      Log.Information($"Bot starting in {(Debugger.IsAttached ? "debug" : "production")} mode");

      GitHub.Init();
      _channelsToConnectTo = await SetChannelsToConnectToAsync().ConfigureAwait(false);

      var emotesThread = new Thread(async () => await Emotes.UpdateEmotes.StartUpdatingEmotes(_channelsToConnectTo).ConfigureAwait(false));
      var authThread = new Thread(async () => await Authentication.StartValidatingTokenAsync().ConfigureAwait(false));
      var remindersThread = new Thread(async () => await Remind.StartCheckingReminders().ConfigureAwait(false));
      var reconnectThread = new Thread(async () => await Bot.StartReconnecting().ConfigureAwait(false));
      emotesThread.Start();
      authThread.Start();
      remindersThread.Start();
      reconnectThread.Start();

      bot = new Bot(_channelsToConnectTo);
      Console.ReadLine();
    }

    
    
    private static async Task<List<string>> SetChannelsToConnectToAsync()
      => await DatabaseConnections.GetConnectedChannels().ConfigureAwait(false);

    private static void BuildSettingsConfing(IConfigurationBuilder builder)
    {
      builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true);
    }
  }
}
