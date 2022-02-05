using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Serilog;
using TwitchBot.Enums;
using TwitchBot.Models;

namespace TwitchBot.Connections
{
  public static class DatabaseConnections
  {
    public static async Task<bool?> IsInCommandsInfo(string commandName)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_IsInCommandsInfo", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("commandName", commandName);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            return Convert.ToBoolean(result);
          }
          catch (Exception e)
          {
            Log.Error("sp_IsInCommandsInfo exception: {ex}", e);
            return null;
          }
        }
      }
    }

    public static async Task AddToCommandsInfo(string commandName)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_AddToCommandsInfo", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("commandName", commandName);

          try
          {
            await com.ExecuteNonQueryAsync();
          }
          catch (Exception e)
          {
            Log.Error("sp_AddToCommandsInfo exception: {ex}", e);
          }
        }
      }
    }

    public static async Task UpdateCommandsInfo(string commandName, ChatMessageModel message)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_UpdateCommandsInfo", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          com.Parameters.AddWithValue("commandName", commandName);
          com.Parameters.AddWithValue("usedWhen", message.TimeStamp);
          com.Parameters.AddWithValue("usedBy", message.Username);
          com.Parameters.AddWithValue("usedIn", message.Channel);

          try
          {
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
          }
          catch (Exception e)
          {
            Log.Error("sp_UpdateCommandsInfo exception: {ex}", e);
            throw;
          }
        }
      }
    }

    public static async Task<decimal> GetTotalCommandsUsed()
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_GetTotalCommandsUsed", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            if (result == null)
              return -1;
            return (decimal)result;
          }
          catch (Exception e)
          {
            Log.Error("sp_GetTotalCommandsUsed exception: {ex}", e);
            return -1;
          }
        }
      }
    }
    
    public static async Task<bool> IsInAfkUsers(string channel, string username)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_IsInAfkUsers", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("afkChannel", channel);
          com.Parameters.AddWithValue("afkUsername", username);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            return (long)result == 1;
          }
          catch (Exception e)
          {
            Log.Error("sp_IsInAfkUsers exception: {ex}", e);
            return false;
          }
        }
      }
    }

    public static async Task AddAfkUser(AfkModel afk)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_AddAfkUser", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          com.Parameters.AddWithValue("afkChannel", afk.Channel);
          com.Parameters.AddWithValue("afkUsername", afk.Username);
          com.Parameters.AddWithValue("afkMessage", afk.Message);
          com.Parameters.AddWithValue("afkSince", afk.AfkSince);

          try
          {
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
          }
          catch (Exception e)
          {
            Log.Error("sp_AddAfkUser exception: {ex}", e);
          }
        }
      }
    }

    public static async Task UpdateAfkUser(AfkModel afk)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_UpdateAfkUser", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          com.Parameters.AddWithValue("afkChannel", afk.Channel);
          com.Parameters.AddWithValue("afkUsername", afk.Username);
          com.Parameters.AddWithValue("afkMessage", afk.Message);
          com.Parameters.AddWithValue("afkSince", afk.AfkSince);
          com.Parameters.AddWithValue("isAfk", afk.IsAfk);

          try
          {
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
          }
          catch (Exception e)
          {
            Log.Error("sp_UpdateAfkUser exception: {ex}", e);
          }
        }
      }
    }

    public static async Task<bool> IsAfk(string channel, string username)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_IsAfk", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("afkChannel", channel);
          com.Parameters.AddWithValue("afkUsername", username);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            if (result != null)
              return (bool)result;
            return false;

          }
          catch (Exception e)
          {
            Log.Error("sp_IsAfk exception: {ex}", e);
            return false;
          }
        }
      }
    }

    public static async Task<AfkModel> GetAfkUser(string channel, string username)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_GetAfkUser", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("afkChannel", channel);
          com.Parameters.AddWithValue("afkUsername", username);

          try
          {
            var reader = await com.ExecuteReaderAsync().ConfigureAwait(false);
            if (!reader.HasRows) 
              return null;

            await reader.ReadAsync();
            return new AfkModel
            {
              Channel = reader.GetString(0),
              Username = reader.GetString(1),
              Message = reader.GetString(2),
              AfkSince = reader.GetDateTime(3),
              IsAfk = reader.GetBoolean(4)
            };

          }
          catch (Exception e)
          {
            Log.Error("sp_GetAfkUser exception: {ex}", e);
            return null;
          }
        }
      }
    }

    public static async Task UpdateOptout(string channel, string username, string command, bool isOptout)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_UpdateOptout", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          com.Parameters.AddWithValue("channelName", Helpers.FirstToUpper(channel));
          com.Parameters.AddWithValue("username", username);
          com.Parameters.AddWithValue("command", command);
          com.Parameters.AddWithValue("isOptout", isOptout);

          try
          {
            await com.ExecuteNonQueryAsync().ConfigureAwait(false);
          }
          catch (Exception e)
          {
            Log.Error("sp_UpdateOptout exception: {ex}", e);
          }
        }
      }
    }

    public static async Task<bool> CheckOptout(string channel, string username, string command)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_CheckOptout", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          com.Parameters.AddWithValue("channelName", Helpers.FirstToUpper(channel));
          com.Parameters.AddWithValue("username", username);
          com.Parameters.AddWithValue("command", command);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            if (result == null)
              return false;
            return (bool)result;
          }
          catch (Exception e)
          {
            Log.Error("sp_CheckOptout exception: {ex}", e);
            return false;
          }
        }
      }
    }

    public static async Task AddReminder(Reminder reminder)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_AddReminder", con))
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

    public static async Task<int> GetReminderId(Reminder reminder)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_GetReminderID", con))
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_DeactivateReminder", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("id", reminder.Id);

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

    public static async Task<bool> AlreadyReminding(Reminder reminder)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_AlreadyReminding", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("fromUsername", reminder.From);
          com.Parameters.AddWithValue("target", reminder.For);

          try
          {
            return Convert.ToBoolean(await com.ExecuteScalarAsync().ConfigureAwait(false));
          }
          catch (Exception e)
          {
            Log.Error("sp_AlreadyReminding exception: {ex}", e);
            return false;
          }
        }
      }
    }

    public static async Task<List<Reminder>> GetActiveTimedReminders()
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_GetActiveTimedReminders", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          try
          {
            var reader = await com.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
              return new List<Reminder>();

            var reminders = new List<Reminder>();
            while (await reader.ReadAsync())
            {
              reminders.Add(new Reminder
              {
                Channel = reader.GetString(0),
                From = reader.GetString(1),
                For = reader.GetString(2),
                Message = reader.GetString(3),
                IsTimed = reader.GetBoolean(5),
                StartTime = reader.GetDateTime(6),
                EndTime = reader.GetDateTime(7),
                Length = TimeSpan.FromSeconds(reader.GetInt32(8)),
                Id = reader.GetInt32(9)
              });
            }

            return reminders;
          }
          catch (Exception e)
          {
            Log.Error("sp_GetActiveTimedReminders exception: {ex}", e);
            return null;
          }
        }
      }
    }

    public static async Task<List<Reminder>> GetActiveUntimedReminders()
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_GetActiveUntimedReminders", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          try
          {
            var reader = await com.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
              return new List<Reminder>();

            var reminders = new List<Reminder>();
            while (await reader.ReadAsync())
            {
              reminders.Add(new Reminder
              {
                Channel = reader.GetString(0),
                From = reader.GetString(1),
                For = reader.GetString(2),
                Message = reader.GetString(3),
                IsTimed = reader.GetBoolean(5),
                StartTime = reader.GetDateTime(6),
                Id = reader.GetInt32(9)
              });
            }

            return reminders;
          }
          catch (Exception e)
          {
            Log.Error("sp_GetActiveUntimedReminders exception: {ex}", e);
            return null;
          }
        }
      }
    }

    public static async Task<bool> IsInUsers(string tableName, string username)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_IsInUsers", con))
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_WriteToUsers", con))
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
          await con.CloseAsync();
        }
      }
    }

    public static async Task UpdateUser(string toUpdate, string tableName, string username, bool? isBanned = null, Permission? permission = null)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_UpdateInUsers", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          com.Parameters.AddWithValue("toUpdate", toUpdate);
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(tableName));
          com.Parameters.AddWithValue("username", username);
          switch (toUpdate)
          {
            case "ban":
              com.Parameters.AddWithValue("isBanned", isBanned);
              com.Parameters.AddWithValue("permission", null);
              break;
            case "perms":
              com.Parameters.AddWithValue("isBanned", null);
              com.Parameters.AddWithValue("permission", (int)permission + 1);
              break;
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_IsBanned", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(tableName));
          com.Parameters.AddWithValue("username", username);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);

            return result != null && Convert.ToBoolean(result);
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_GetPermission", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(tableName));
          com.Parameters.AddWithValue("username", username);

          try
          {
            var result = await com.ExecuteScalarAsync().ConfigureAwait(false);
            if (result == null)
              return Permission.Regular;
            if (Enum.TryParse(Convert.ToString(result), out bool _))
              return (Permission)Enum.Parse(typeof(Credentials), Convert.ToString(result)!);
            return Permission.Regular;
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Trying to write emote(s) to db.");

        con.Open();
        await using (var com = new MySqlCommand("sp_WriteEmote", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          foreach (var emote in emotes)
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        con.Open();
        await using (var com = new MySqlCommand("sp_UpdateEmote", con))
        {
          com.CommandType = CommandType.StoredProcedure;

          foreach (var emote in emotes)
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
      var emotes = new List<EmoteModel>();
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Getting emotes from db.");

        con.Open();
        await using (var com = new MySqlCommand("sp_GetEmotes", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("tableName", Helpers.FirstToUpper(channel));

          await using (var reader = await com.ExecuteReaderAsync().ConfigureAwait(false))
          {
            if (!reader.HasRows)
            {
              Log.Warning("{table} has no rows.", Helpers.FirstToUpper(channel) + "Emotes");
              return new List<EmoteModel>();
            }
            while (await reader.ReadAsync())
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Getting last added emotes");

        con.Open();
        await using (var com = new MySqlCommand("sp_AddedEmotes", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("channel", Helpers.FirstToUpper(channel));

          await using (var reader = await com.ExecuteReaderAsync())
          {
            var emotes = new List<EmoteModel>();
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Getting last removed emotes");

        con.Open();
        await using (var com = new MySqlCommand("sp_RemovedEmotes", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("channel", Helpers.FirstToUpper(channel));

          await using (var reader = await com.ExecuteReaderAsync())
          {
            var emotes = new List<EmoteModel>();
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
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Writing suggestion ({suggestion})", msg);

        con.Open();
        await using (var com = new MySqlCommand("sp_WriteSuggestion", con))
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
          await con.CloseAsync();
        }
      }
    }

    public static async Task WriteMessage(ChatMessageModel msg)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Writing \"{message}\" to db.", msg.Message);

        con.Open();
        await using (var com = new MySqlCommand("sp_WriteMessage", con))
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
          await con.CloseAsync();
        }
      }
    }

    public static async Task<List<string>> GetConnectedChannels()
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Information("Getting channels to connect to.");

        con.Open();
        await using (var command = new MySqlCommand("sp_GetConnectedChannels", con))
        {
          command.CommandType = CommandType.StoredProcedure;
          await using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
          {
            var channels = new List<string>();
            while (await reader.ReadAsync())
            {
              channels.Add(reader.GetString(0));
            }

            await con.CloseAsync();
            Log.Debug("Successfully got channels to connect to.");
            return channels;
          }
        }
      }
    }

    public static async Task<int> GetConnectedChannelId(string channel)
    {
      await using (var con = new MySqlConnection(SecretsConfig.Credentials.ConnectionString))
      {
        Log.Debug("Trying to get ID for {channel}", channel);

        con.Open();
        await using (var com = new MySqlCommand("sp_GetIDByConnectedChannel", con))
        {
          com.CommandType = CommandType.StoredProcedure;
          com.Parameters.AddWithValue("searchChannel", Helpers.FirstToUpper(channel));

          await using (var reader = await com.ExecuteReaderAsync().ConfigureAwait(false))
          {
            if (await reader.ReadAsync())
            {
              Log.Debug("Got ID for {channel}", channel);
              return (int)reader[0];
            }

            Log.Error("Couldn't get ID for {channel}", channel);
            return -1;
          }
        }
      }
    }
  }
}
