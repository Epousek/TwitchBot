using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Serilog;
using TwitchBot.src.Enums;
using TwitchBot.src.Models;

namespace TwitchBot.src.Connections
{
  public static class DatabaseConnections
  {
    public static async Task AddReminder(Reminder reminder)
    {
      using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (var com = new MySqlCommand("sp_AddReminder", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          com.Parameters.AddWithValue("channelWhere", reminder.Channel);
          com.Parameters.AddWithValue("fromUsername", reminder.From);
          com.Parameters.AddWithValue("forUsername", reminder.For);
          com.Parameters.AddWithValue("message", reminder.Message);
          com.Parameters.AddWithValue("isTimed", reminder.IsTimed);
          com.Parameters.AddWithValue("startTime", reminder.StartTime);
          if (reminder.IsTimed)
          {
            com.Parameters.AddWithValue("endTime", reminder.EndTime);
            com.Parameters.AddWithValue("lengthInSeconds", ((TimeSpan)reminder.Length).TotalSeconds);
          }
          else
          {
            com.Parameters.AddWithValue("endTime", null);
            com.Parameters.AddWithValue("lengthInSeconds", null);
          }

          try
          {
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
          }
          catch (Exception e)
          {
            Log.Error("sp_AddReminder exception: {ex}", e);
          }
        }
      }
    }

    public static async Task<int> GetReminderID(Reminder reminder)
    {
      using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (var com = new MySqlCommand("sp_GetReminderID", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("username", reminder.From);

          try
          {
            return (int)await com.ExecuteScalarAsync().ConfigureAwait(false);
          }
          catch (Exception e)
          {
            Log.Error("sp_GetReminderID exception: {ex}", e);
            return -1;
          }
        }
      }
    }

    public static async Task DeactivateReminder(Reminder reminder)
    {
      using(var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (var com = new MySqlCommand("sp_DeactivateReminder", con))
        {
          com.CommandType= CommandType.StoredProcedure;
          com.Parameters.AddWithValue("id", reminder.ID);

          try
          {
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
          }
          catch (Exception e)
          {
            Log.Error("sp_DeactivateReminder exception: {ex}", e);
          }
        }
      }
    }

    public static async Task<bool> IsInUsers(string tableName, string username)
    {
      using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (var com = new MySqlCommand("sp_IsInUsers", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(tableName));
          com.Parameters.AddWithValue("username", username);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            await con.CloseAsync().ConfigureAwait(false);
            return Convert.ToBoolean(result);
          }
          catch (Exception e)
          {
            Log.Error("sp_IsInUser exception: {ex}", e);
            return false;
          }
        }
      }
    }

    public static async Task WriteToUsers(string tableName, string username)
    {
      using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (var com = new MySqlCommand("sp_WriteToUsers", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(tableName));
          com.Parameters.AddWithValue("username", username);

          try
          {
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
            Log.Debug("Wrote {user} to {channel}", username, tableName);
          }
          catch (Exception e)
          {
            Log.Error("sp_AddToUsers exception: {ex}", e);
          }
          con.Close();
        }
      }
    }

    public static async Task UpdateUser(string toUpdate, string tableName, string username, bool? isBanned = null, Permission? permission = null)
    {
      using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (var com = new MySqlCommand("sp_UpdateInUsers", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          com.Parameters.AddWithValue("toUpdate", toUpdate);
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(tableName));
          com.Parameters.AddWithValue("username", username);
          if (toUpdate == "ban")
          {
            com.Parameters.AddWithValue("isBanned", isBanned);
            com.Parameters.AddWithValue("permission", null);

          }
          if (toUpdate == "perms")
          {
            com.Parameters.AddWithValue("isBanned", null);
            com.Parameters.AddWithValue("permission", (int)permission + 1);
          }

          try
          {
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
          }
          catch (Exception e)
          {
            Log.Error("sp_UpdateInUsers exception: {ex}", e);
          }
        }
      }
    }

    public static async Task<bool> IsBanned(string tableName, string username)
    {
      using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (var com = new MySqlCommand("sp_IsBanned", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(tableName));
          com.Parameters.AddWithValue("username", username);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            if (result != null)
              return Convert.ToBoolean(result);
            else
              return false;
          }
          catch (Exception e)
          {
            Log.Error("sp_IsBanned exception: {ex}", e);
            return false;
          }
        }
      }
    }

    public static async Task<Permission?> GetPermission(string tableName, string username)
    {
      using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        using (var com = new MySqlCommand("sp_GetPermission", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(tableName));
          com.Parameters.AddWithValue("username", username);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            return (Permission)Enum.Parse(typeof(Permission), Convert.ToString(result));
          }
          catch (Exception e)
          {
            Log.Error("sp_GetPermission exception: {ex}", e);
            return Permission.Regular;
          }
        }
      }
    }

    public static async Task WriteEmotes(string channel, List<EmoteModel> emotes)
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Trying to write emote(s) to db.");

        con.Open();
        using (MySqlCommand com = new("sp_WriteEmote", con))
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
        using (MySqlCommand com = new("sp_UpdateEmote", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          foreach (EmoteModel emote in emotes)
          {
            if ((emote.IsActive && emote.Added == null) || (!emote.IsActive && emote.Removed == null))
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
              com.Parameters.AddWithValue("service", emote.Service);

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
              return new List<EmoteModel>();
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

    public static async Task<List<EmoteModel>> GetLastAddedEmotes(string channel)
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Getting last added emotes");

        con.Open();
        using (MySqlCommand com = new("sp_AddedEmotes", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("channel", Helpers.FirstToUpper(channel));

          using (var reader = await com.ExecuteReaderAsync())
          {
            List<EmoteModel> emotes = new();
            if (!reader.HasRows)
            {
              Log.Warning("Couldn't retrieve data from {0}Emotes (it may not have any).", channel);
              return emotes;
            }
            while (await reader.ReadAsync())
            {
              emotes.Add(new EmoteModel
              {
                Name = reader.GetString(0),
                Service = reader.GetString(1),
                Added = reader.GetDateTime(2),
                Removed = await reader.IsDBNullAsync(3).ConfigureAwait(false) ? null : reader.GetDateTime(3),
                IsActive = reader.GetBoolean(4)
              });
            }

            return emotes;
          }
        }
      }
    }

    public static async Task<List<EmoteModel>> GetLastRemovedEmotes(string channel)
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Getting last removed emotes");

        con.Open();
        using (MySqlCommand com = new("sp_RemovedEmotes", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("channel", Helpers.FirstToUpper(channel));

          using (var reader = await com.ExecuteReaderAsync())
          {
            List<EmoteModel> emotes = new();
            if (!reader.HasRows)
            {
              Log.Warning("Couldn't retrieve data from {0}Emotes (it may not have any).", channel);
              return emotes;
            }
            while (await reader.ReadAsync())
            {
              emotes.Add(new EmoteModel
              {
                Name = reader.GetString(0),
                Service = reader.GetString(1),
                Added = reader.GetDateTime(2),
                Removed = await reader.IsDBNullAsync(3).ConfigureAwait(false) ? null : reader.GetDateTime(3),
                IsActive = reader.GetBoolean(4)
              });
            }

            return emotes;
          }
        }
      }
    }

    public static async Task WriteSuggestion(ChatMessageModel msg)
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Writing suggestion ({suggestion})", msg);

        con.Open();
        using (MySqlCommand com = new("sp_WriteSuggestion", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("timeStamp", msg.TimeStamp);
          com.Parameters.AddWithValue("user", msg.Username);
          com.Parameters.AddWithValue("suggestion", msg.Message);

          try
          {
            await com.ExecuteNonQueryAsync();
          }
          catch (Exception e)
          {
            Log.Error("Couldn't write suggestion to db: {error}: {errorMessage}", e, e.Message);
          }
          con.Close();
        }
      }
    }

    public static async Task WriteMessage(ChatMessageModel msg)
    {
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Writing \"{message}\" to db.", msg.Message);

        con.Open();
        using (MySqlCommand com = new("sp_WriteMessage", con))
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
            BotInfo.MessagesLoggedSinceStart++;
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
      using (MySqlConnection con = new(SecretsConfig.Credentials.ConnectionString))
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

          using (var reader = await com.ExecuteReaderAsync().ConfigureAwait(false))
          {
            if (reader.Read())
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
