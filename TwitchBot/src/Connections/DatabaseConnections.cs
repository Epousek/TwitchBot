using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Serilog;

namespace TwitchBot.src.Connections
{
  public static class DatabaseConnections
  {
    public static async Task WriteMessage(string channel, string username, string message, DateTime timeStamp)
    {
      using (MySqlConnection con = new (SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Writing \"{message}\" to db.", message);

        con.Open();
        using (MySqlCommand com = new ("sp_WriteMessage", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", char.ToUpper(channel[0]) + channel[1..]);
          com.Parameters.AddWithValue(nameof(username), username);
          com.Parameters.AddWithValue(nameof(message), message);
          com.Parameters.AddWithValue("messageTimeStamp", timeStamp);

          try
          {
            int x = await com.ExecuteNonQueryAsync().ConfigureAwait(false);
            Log.Debug("Write successful.");
          }
          catch (Exception e)
          {
            Log.Error(e, "Write unsuccessful.");
          }
          con.Close();
        }
      }
    }

    public static async Task<List<string>> GetConnectedChannels()
    {
      using (MySqlConnection con = new (SecretsConfig.Credentials.ConnectionString))
      {
        Log.Information("Getting channels to connect to.");

        con.Open();
        using (MySqlCommand command = new("sp_GetConnectedChannels", con))
        {
          command.CommandType = CommandType.StoredProcedure;
          using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
          {
            List<string> channels = new();
            while (reader.Read())
            {
              for (int i = 0; i < reader.FieldCount; i++)
              {
                channels.Add((string)reader[i]);
              }
            }

            con.Close();
            Log.Debug("Successfully got channels to connect to.");
            return channels;
          }
        }
      }
    }
  }
}
