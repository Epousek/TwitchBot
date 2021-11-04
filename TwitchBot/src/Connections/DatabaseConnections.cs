using System.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace TwitchBot.src.Connections
{
  public static class DatabaseConnections
  {
    public static async Task WriteMessage(string channel, string username, string message, DateTime timeStamp)
    {
      using (MySqlConnection con = new (SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (MySqlCommand com = new ("sp_WriteMessage", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", char.ToUpper(channel[0]) + channel.Substring(1));
          com.Parameters.AddWithValue("username", username);
          com.Parameters.AddWithValue("message", message);
          com.Parameters.AddWithValue("messageTimeStamp", timeStamp);

          try
          {
            
            int x = await com.ExecuteNonQueryAsync();
          }
          catch
          {

          }
          con.Close();
        }
      }
    }

    public static async Task<string> GetMessage()
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (MySqlCommand com = new ("SELECT message FROM EpousekLogs ORDER BY messageTimeStamp LIMIT 1", con))
        {
          var reader = await com.ExecuteReaderAsync();
          if (reader.Read())
            return (string)reader[0];
          else
            return null;
        }
      }
    }
    
    public static async Task<List<string>> GetConnectedChannels()
    {
      using (MySqlConnection con = new (SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (MySqlCommand command = new MySqlCommand("sp_GetConnectedChannels", con))
        {
          command.CommandType = CommandType.StoredProcedure;
          using (var reader = await command.ExecuteReaderAsync())
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
            return channels;
          }
        }
      }
    }

  }
}
