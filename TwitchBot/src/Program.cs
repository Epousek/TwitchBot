using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TwitchBot.src.Connections;
using Serilog;
using Serilog.Sinks.MariaDB.Extensions;
using System.Diagnostics;

namespace TwitchBot.src
{
  class Program
  {
    static List<string> channelsToConnectTo;

    static async Task Main()
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
        connectionString: SecretsConfig.Credentials.ConnectionString,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        autoCreateTable: true,
        tableName: "Logs"
        )
        .CreateLogger();

      Log.Information($"Bot starting in {(Debugger.IsAttached ? "debug" : "production")} mode");

      channelsToConnectTo = await SetChannelsToConnectToAsync().ConfigureAwait(false);

      Thread emotesThread = new(async () => await Emotes.UpdateEmotes.StartUpdatingEmotes(channelsToConnectTo).ConfigureAwait(false));
      Thread authThread = new (async () => await Authentication.StartValidatingTokenAsync().ConfigureAwait(false));
      emotesThread.Start();
      authThread.Start();

      Bot bot = new(channelsToConnectTo);
      Console.ReadLine();
    }

    private static async Task<List<string>> SetChannelsToConnectToAsync()
      => await DatabaseConnections.GetConnectedChannels().ConfigureAwait(false);

    private static void BuildSettingsConfing(IConfigurationBuilder builder)
    {
      builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    }
  }
}
