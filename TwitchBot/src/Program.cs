using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TwitchBot.src.Connections;
using Serilog;
using Serilog.Sinks.MariaDB;
using Serilog.Sinks.MariaDB.Extensions;
using System.Diagnostics;

namespace TwitchBot.src
{
  class Program
  {
    static async Task Main()
    {
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
#if DEBUG
        tableName: "LogsTest"
#else
        tableName: "Logs"
#endif
        )
        .CreateLogger();

      Log.Information($"Bot starting in {(Debugger.IsAttached ? "debug" : "production")} mode");

      Thread authThread = new (async () => await Authentication.StartValidatingTokenAsync().ConfigureAwait(false));
      authThread.Start();

      Bot bot = new(await Task.Run(SetChannelsToConnectToAsync).ConfigureAwait(false));
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
