using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Models;

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
      commandInstances.Add("test", new TestCommandWithArgument());
    }

    public async Task CheckIfCommandAsync(ChatMessageModel message)
    {
      message.Message = message.Message[1..];

      KeyValuePair<string, ICommand> command;
      IEnumerable<KeyValuePair<string, ICommand>> commands = commandInstances.Where(c => message.Message.StartsWith(c.Key, StringComparison.OrdinalIgnoreCase));

      if (commands?.Any() == true)
      {
        command = commands.First();
        await command.Value.UseCommandAsync(message);
        BotInfo.CommandsUsedSinceStart++;
      }
      else
      {
        commands = commandInstances.Where(c =>
        {
          if(c.Value.Aliases.Length > 0)
          {
            foreach (string alias in c.Value.Aliases)
            {
              if(message.Message.StartsWith(alias, StringComparison.OrdinalIgnoreCase))
                return true;
            }
          }
          return false;
        });

        if (commands?.Any() == true)
        {
          command = commands.First();
          await command.Value.UseCommandAsync(message);
          BotInfo.CommandsUsedSinceStart++;
        }
        else
        {
          Log.Debug("Není to command!");
        }
      }
    }
  }
}
