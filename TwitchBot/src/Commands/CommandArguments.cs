using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchBot.src.Models;

namespace TwitchBot.src.Commands
{
  public class CommandArguments
  {
    readonly private string message;

    public CommandArguments(ChatMessageModel m)
    {
      if (m.Message.Contains(' '))
      {
        message = m.Message.Remove(0, m.Message.IndexOf(' ') + 1); //remove the command
        message = message.TrimStart();
      }
      else
      {
        message = string.Empty;
      }
    }

    public List<string> GetOneArgument()
    {
      return new List<string>() { message };
    }

    public List<string> GetTwoArguments()
    {
      List<string> args = new List<string>(Regex.Split(message, "(?= )"));
      int count = args.Count;
      if (count < 2)
        return new List<string>();
      for (int i = 2; i < count; i++)
      {
        args[1] += args[2];
        args.RemoveAt(2);
      }

      return args;
    }
  }
}
