using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Serilog;
using TwitchBot.src.Models;
using System.Data.Common;

namespace TwitchBot.src.Connections
{
  public static class DatabaseConnections
  {
    public static async Task WriteEmotes(string channel, List<EmoteModel> emotes)
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Trying to write emote(s) to db.");

        con.Open();
        using(MySqlCommand com = new("sp_WriteEmote", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          foreach (EmoteModel emote in emotes)
          {
            com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(channel));
            com.Parameters.AddWithValue("emoteName", emote.Name);
            com.Parameters.AddWithValue("service", emote.Service);
            com.Parameters.AddWithValue("isActive", true);
            com.Parameters.AddWithValue("added", emote.Added);
            com.Parameters.AddWithValue("removed", emote.Removed);

            try
            {
              await com.ExecuteNonQueryAsync().ConfigureAwait(false);
              Log.Debug("Emote({emote}) write successful.", emote.Name);
            }
            catch (DbException e)
            {
              Log.Error(e, "Emote({emote}) write unsuccessful: {ex}", emote.Name, e.Message);
            }
            com.Parameters.Clear();
          }
        }
      }
    }

    public static async Task UpdateEmotes(string channel, List<EmoteModel> emotes)
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using(MySqlCommand com = new("sp_UpdateEmote", con))
        {
          foreach (EmoteModel emote in emotes)
          {
            if((emote.IsActive && emote.Added == null) || (!emote.IsActive && emote.Removed == null))
            {
              Log.Error("{emote} is {isActive}, but doesn't have a time stamp. Skipping.", emote.Name, emote.IsActive);
            }
            else
            {
              Log.Debug("Updating {emote} to {isActive} ({timeStamp}).", emote.Name, emote.IsActive, emote.IsActive ? emote.Added : emote.Removed);

              com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(channel));
              com.Parameters.AddWithValue("emoteName", emote.Name);
              com.Parameters.AddWithValue("isActive", emote.IsActive);
              com.Parameters.AddWithValue("added", emote.Added);
              com.Parameters.AddWithValue("removed", emote.Removed);
              try
              {
                await com.ExecuteNonQueryAsync().ConfigureAwait(false);
                Log.Debug("Updated {emote} to {isActive}.", emote.Name, emote.IsActive);
              }
              catch (DbException e)
              {
                Log.Error(e, "Couldn't update {emote}: {ex}", emote.Name, e.Message);
              }

              com.Parameters.Clear();
            }
          }
        }
      }
    }

    public static async Task<List<EmoteModel>> GetEmotes(string channel)
    {
      List<EmoteModel> emotes = new();
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Getting emotes from db.");

        con.Open();
        using (MySqlCommand com = new("sp_GetEmotes", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(channel));
          
          using (var reader = await com.ExecuteReaderAsync().ConfigureAwait(false))
          {
            if (!reader.HasRows)
            {
              Log.Warning("{table} has no rows.", Helpers.FirstToUpper(channel) + "Emotes");
              return null;
            }
            while (reader.Read())
            {
              emotes.Add(new EmoteModel
              {
                Name = reader.GetString(0),
                Service = reader.GetString(1),
                Added = await reader.IsDBNullAsync(2).ConfigureAwait(false) ? null : reader.GetDateTime(2),
                Removed = await reader.IsDBNullAsync(3).ConfigureAwait(false) ? null : reader.GetDateTime(3),
                IsActive = reader.GetBoolean(4)
              });
            }

            Log.Debug("Successfully got emotes from db.");
            return emotes;
          }
        }
      }
    }

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
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
            Log.Debug("Message write successful.");
          }
          catch (Exception e)
          {
            Log.Error(e, "Message write unsuccessful: {ex}", e.Message);
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
              channels.Add(reader.GetString(0));
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
