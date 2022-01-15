using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Models;
using TwitchBot.src.Interfaces;
using TwitchBot.src.Connections;

namespace TwitchBot.src.Commands
{
  public class CommandGetter
  {
    public Dictionary<string, ICommand> commandInstances = new();

    public CommandGetter()
    {
      commandInstances.Clear();
      commandInstances.Add("Emotes", new Emotes());
      commandInstances.Add("Removed", new Removed());
      commandInstances.Add("Suggest", new Suggest());
      commandInstances.Add("Info", new Info());
      commandInstances.Add("Commands", new Commands());
      commandInstances.Add("About", new About());
      commandInstances.Add("Help", new Help());
      commandInstances.Add("Ban", new Ban());
    }

    public async Task CheckIfCommandAsync(ChatMessageModel message)
    {
      message.Message = message.Message[1..];

      ICommand command;
      IEnumerable<KeyValuePair<string, ICommand>> commands = commandInstances.Where(c => message.Message.StartsWith(c.Key, StringComparison.OrdinalIgnoreCase));

      if (commands?.Any() == true)
      {
        command = commands.First().Value;
        await TryToUseCommand(command, message).ConfigureAwait(false);
      }
      else
      {
        commands = commandInstances.Where(c =>
        {
          if (c.Value.Aliases.Length > 0)
          {
            foreach (string alias in c.Value.Aliases)
            {
              if (message.Message.StartsWith(alias, StringComparison.OrdinalIgnoreCase))
                return true;
            }
          }
          return false;
        });

        if (commands?.Any() == true)
        {
          command = commands.First().Value;
          await TryToUseCommand(command, message).ConfigureAwait(false);
        }
        else
        {
          Log.Debug("Není to command!");
        }
      }
    }

    private async Task TryToUseCommand(ICommand command, ChatMessageModel message)
    {
      if (!IsOnCooldown(command))
      {
        if (await UsableBan(command, message))
        {
          await command.UseCommandAsync(message).ConfigureAwait(false);
          command.LastUsed = DateTime.Now;
          command.TimesUsedSinceRestart++;
          BotInfo.CommandsUsedSinceStart++;
        }
        return;
      }
    }

    private async Task<bool> UsableBan(ICommand command, ChatMessageModel message)
    {
      if (command.UsableByBanned) { return true; }
      else
      {
        if (!await DatabaseConnections.IsInUsers(message.Channel, message.Username))
        {
          return true;
        }
        else if (!await DatabaseConnections.IsBanned(message.Channel, message.Username))
        {
          return true;
        }
        else
        {
          Bot.WriteMessage($"@{message.Username} Bohužel máš zakázáno používat příkazy :/ Pokud si myslíš, žes byl zabanovanej neprávem/omylem, napiš whisp @epousek", message.Channel);
          return false;
        }
      }
    }

    private static bool IsOnCooldown(ICommand command)
      => (DateTime.Now - command.LastUsed).TotalSeconds < command.Cooldown.TotalSeconds;
  }
}
