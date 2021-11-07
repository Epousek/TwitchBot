using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Serilog;
using TwitchBot.src.Models;

namespace TwitchBot.src.Connections
{
  public static class DatabaseConnections
  {
    public static async Task WriteMessage(ChatMessageModel msg)
    {
      using (MySqlConnection con = new (SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Writing \"{message}\" to db.", msg.Message);

        con.Open();
        using (MySqlCommand com = new ("sp_WriteMessage", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(msg.Channel));
          com.Parameters.AddWithValue("username", msg.Username);
          com.Parameters.AddWithValue("message", msg.Message);
          com.Parameters.AddWithValue("messageTimeStamp", msg.TimeStamp);

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

    public static async Task<int> GetConnectedChannelID(string channel)
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Trying to get ID for {channel}", channel);

        con.Open();
        using (MySqlCommand com = new("sp_GetIDByConnectedChannel", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("searchChannel", Helpers.FirstToUpper(channel));

          using(var reader = await com.ExecuteReaderAsync().ConfigureAwait(false))
          {
            if(reader.Read())
            {
              Log.Debug("Got ID for {channel}", channel);
              return (int)reader[0];
            }
            else
            {
              Log.Error("Couldn't get ID for {channel}", channel);
              return -1;
            }
          }
        }
      }
    }
  }
}
