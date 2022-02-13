using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using TwitchBot.Connections;
using TwitchBot.Interfaces;
using TwitchBot.Models;
using TwitchBot.Commands.Status;

namespace TwitchBot.Commands
{
  public class CommandGetter
  {
    public readonly Dictionary<string, ICommand> CommandInstances = new();

    public CommandGetter()
    {
      CommandInstances.Clear();
      CommandInstances.Add("Emotes", new Emotes());
      CommandInstances.Add("Removed", new Removed());
      CommandInstances.Add("Remind", new Remind());
      CommandInstances.Add("Afk", new Afk());
      CommandInstances.Add("Gn", new Gn());
      CommandInstances.Add("Food", new Food());
      CommandInstances.Add("School", new School());
      CommandInstances.Add("Status", new CheckStatus());
      CommandInstances.Add("Suggest", new Suggest());
      CommandInstances.Add("Help", new Help());
      CommandInstances.Add("Commands", new Commands());
      CommandInstances.Add("Info", new Info());
      CommandInstances.Add("About", new About());
      CommandInstances.Add("Ban", new Ban());
      CommandInstances.Add("Unban", new Unban());
      CommandInstances.Add("Admin", new Admin());
      CommandInstances.Add("Unadmin", new Unadmin());
      CommandInstances.Add("Optout", new Optout());
      CommandInstances.Add("Optin", new Optin());
    }

    public async Task CheckIfCommandAsync(ChatMessageModel message)
    {
      message.Message = Bot.Prefix == "$" ? message.Message[1..] : message.Message[2..];

      ICommand command;
      var commands = CommandInstances.Where(c => message.Message.StartsWith(c.Key, StringComparison.OrdinalIgnoreCase));

      var commandList = commands.ToList();
      if (commandList.Any())
      {
        command = commandList.First().Value;
        await TryToUseCommand(command, message).ConfigureAwait(false);
      }
      else
      {
        commands = CommandInstances.Where(c =>
        {
          return c.Value.Aliases.Length > 0 &&
                 c.Value.Aliases.Any(alias => message.Message.StartsWith(alias,
                   StringComparison.OrdinalIgnoreCase));
        });

        commandList = commands.ToList();
        if (commandList.Any())
        {
          command = commandList.First().Value;
          await TryToUseCommand(command, message).ConfigureAwait(false);
        }
        else
        {
          Log.Debug("Není to command!");
        }
      }
    }

    private static async Task TryToUseCommand(ICommand command, ChatMessageModel message)
    {
      if (command.Permission <= await DatabaseConnections.GetPermission(message.Channel, message.Username).ConfigureAwait(false))
      {
        if (await UsableBan(command, message).ConfigureAwait(false))
        {
          if (!IsOnCooldown(command))
          {
            await command.UseCommandAsync(message).ConfigureAwait(false);
            await IncrementCommand(command, message);
          }
        }
      }
      else
      {
        Bot.WriteMessage("@" + message.Username + " Nemáš dostatečná oprávnění pro tento příkaz.", message.Channel);
      }
    }

    private static async Task<bool> UsableBan(ICommand command, ChatMessageModel message)
    {
      if (command.UsableByBanned) 
        return true;
      if (!await DatabaseConnections.IsInUsers(message.Channel, message.Username))
        return true;
      if (!await DatabaseConnections.IsBanned(message.Channel, message.Username))
        return true;
      
      Bot.WriteMessage($"@{message.Username} Bohužel máš zakázáno používat příkazy :/ Pokud si myslíš, žes byl zabanovanej neprávem/omylem, napiš whisp @epousek", message.Channel);
      return false;
    }

    private static async Task IncrementCommand(ICommand command, ChatMessageModel message)
    {
      command.LastUsed = DateTime.Now;
      command.TimesUsedSinceRestart++;
      BotInfo.CommandsUsedSinceStart++;

      var isInCommandsInfo = await DatabaseConnections.IsInCommandsInfo(command.Name).ConfigureAwait(false);
      
      if (isInCommandsInfo == null)
        return;
      if (!(bool)isInCommandsInfo)
        await DatabaseConnections.AddToCommandsInfo(command.Name).ConfigureAwait(false);

      await DatabaseConnections.UpdateCommandsInfo(command.Name, message);
    }

    private static bool IsOnCooldown(ICommand command)
      => (DateTime.Now - command.LastUsed).TotalSeconds < command.Cooldown.TotalSeconds;
  }
}
