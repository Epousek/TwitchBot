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

namespace TwitchBot.src
{
  class Program
  {
    static List<string> channelsToConnectTo;

    static async Task Main(string[] args)
    {
      SecretsConfig.SetConfig();

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

      Thread authThread = new (async () => await Authentication.StartValidatingTokenAsync());
      authThread.Start();

      Bot bot = new(await Task.Run(SetChannelsToConnectToAsync));
      Console.ReadLine();
    }

    static async Task<List<string>> SetChannelsToConnectToAsync()
      => await DatabaseConnections.GetConnectedChannels();

    static void BuildSettingsConfing(IConfigurationBuilder builder)
    {
      builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    }
  }
}
