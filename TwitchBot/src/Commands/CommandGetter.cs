using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Models;
using TwitchBot.src.Interfaces;

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
    }

    public async Task CheckIfCommandAsync(ChatMessageModel message)
    {
      message.Message = message.Message[1..];

      ICommand command;
      IEnumerable<KeyValuePair<string, ICommand>> commands = commandInstances.Where(c => message.Message.StartsWith(c.Key, StringComparison.OrdinalIgnoreCase));

      if (commands?.Any() == true)
      {
        command = commands.First().Value;
        if (!IsOnCooldown(command))
        {
          await command.UseCommandAsync(message);
          command.LastUsed = DateTime.Now;
          command.TimesUsedSinceRestart++;
          BotInfo.CommandsUsedSinceStart++;
        }
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
          if (!IsOnCooldown(command))
          {
            await command.UseCommandAsync(message);
            command.LastUsed = DateTime.Now;
            command.TimesUsedSinceRestart++;
            BotInfo.CommandsUsedSinceStart++;
          }
        }
        else
        {
          Log.Debug("Není to command!");
        }
      }
    }

    private static bool IsOnCooldown(ICommand command)
      => (DateTime.Now - command.LastUsed).TotalSeconds < command.Cooldown.TotalSeconds;
  }
}
