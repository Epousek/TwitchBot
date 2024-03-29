﻿using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Humanizer.Localisation;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands.Status
{
  internal class Afk : ICommand
  {
    public string Name { get; } = nameof(Afk);
    public string AboutCommand { get; } = "Budeš označen jako AFK.";
    public string HelpMessage { get; } = "$afk *zpráva*";
    public string[] Aliases { get; } = Array.Empty<string>();
    public Permission Permission { get; } = Permission.Regular;
    public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(3);
    public DateTime LastUsed { get; set; }
    public bool OfflineOnly { get; } = false;
    public bool UsableByBanned { get; } = false;
    public bool Optoutable { get; } = false;
    public int TimesUsedSinceRestart { get; set; }
    public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public async Task UseCommandAsync(ChatMessageModel message)
    {
      var statusEnum = await DatabaseConnections.GetStatusEnumOnly(message.Channel, message.Username).ConfigureAwait(false);
      StatusModel status;
      
      switch (statusEnum)
      {
        case Enums.Status.None:
          status = GetSetStatus.CreateStatus(message, Enums.Status.Afk);
          Bot.WriteMessage(
            string.IsNullOrEmpty(status.Message)
              ? $"{message.Username} je nyní afk."
              : $"{message.Username} je nyní afk: {status.Message}", message.Channel);
          break;
        case Enums.Status.Afk:
          Bot.WriteMessage($"@{message.Username} Už jsi afk.", message.Channel); //TODO: update status
          return;
        default:
          status = GetSetStatus.CreateStatus(message, Enums.Status.Afk);
          Bot.WriteMessage(GetSetStatus.StatusChange(message.Username, statusEnum, Enums.Status.Afk), message.Channel);
          break;
      }

      if (await DatabaseConnections.IsInUserStatuses(message.Channel, message.Username).ConfigureAwait(false))
        await DatabaseConnections.UpdateUserStatus(status).ConfigureAwait(false);
      else
        await DatabaseConnections.AddUserStatus(status).ConfigureAwait(false);
      await DatabaseConnections.SetLastStatus(status.Channel, status.Username, Enums.Status.Afk);
    }
  }
}
