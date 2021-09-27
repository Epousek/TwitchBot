using System.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TwitchBot.src.Connections
{
  public static class DatabaseConnections
  {
    public static async Task<List<string>> GetConnectedChannels()
    {
      using (MySqlConnection connection = new MySqlConnection(Config.Credentials.ConnectionString))
      {
        connection.Open();
        using (MySqlCommand command = new MySqlCommand("sp_GetConnectedChannels", connection))
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

            connection.Close();
            return channels;
          }
        }
      }
    }

  }
}
