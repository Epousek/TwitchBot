using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  class CommandGetter
  {
    private Dictionary<string, ICommand> commandInstances = new();

    public CommandGetter()
    {
      commandInstances.Add("Emotes", new Emotes());
    }

    public async Task CheckIfCommandAsync(ChatMessageModel message)
    {
      message.Message = message.Message[1..];

      KeyValuePair<string, ICommand> command;
      IEnumerable<KeyValuePair<string, ICommand>> commands;
      commands = commandInstances.Where(c => message.Message.StartsWith(c.Value.Name, StringComparison.OrdinalIgnoreCase));

      if (commands?.Any() == true)
      {
        command = commands.First();
        await command.Value.UseCommandAsync(message);
      }
      else
      {
        commands = commandInstances.Where(c =>
        {
          foreach (string alias in c.Value.Aliases)
          {
            if(message.Message.StartsWith(alias, StringComparison.OrdinalIgnoreCase));
              return true;
          }
          return false;
        });

        if (commands?.Any() == true)
        {
          command = commands.First();
          await command.Value.UseCommandAsync(message);
        }
        else
        {
          Log.Debug("Není to command!");
        }
      }
    }
  }
}
