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

    public List<string> GetXArguments(int argCount)
    {
      var args = new List<string>(Regex.Split(message, "(?= )"));
      int count = args.Count;
      if (count < argCount)
        return new List<string>();
      for (int i = argCount; i < count; i++)
      {
        args[argCount - 1] += args[argCount];
        args.RemoveAt(argCount);
      }
      return args;
    }
  }
}
