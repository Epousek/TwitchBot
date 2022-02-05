using System.Collections.Generic;
using System.Text.RegularExpressions;
using TwitchBot.Models;

namespace TwitchBot.Commands
{
  public class CommandArguments
  {
    private readonly string _message;

    public CommandArguments(ChatMessageModel m)
    {
      if (m.Message.Contains(' '))
      {
        _message = m.Message.Remove(0, m.Message.IndexOf(' ') + 1); //remove the command
        _message = _message.TrimStart();
      }
      else
      {
        _message = string.Empty;
      }
    }

    public List<string> GetOneArgument()
      => string.IsNullOrEmpty(_message) ? new List<string>() : new List<string> { _message };

    public List<string> GetXArguments(int argCount)
    {
      var args = new List<string>(Regex.Split(_message, "(?= )"));
      var count = args.Count;
      //if (count < argCount)
      //  return new List<string>();
      for (int i = argCount; i < count; i++)
      {
        args[argCount - 1] += args[argCount];
        args.RemoveAt(argCount);
      }
      for (int i = 0; i < args.Count; i++)
      {
        args[i] = args[i].TrimStart();
      }
      while (args.Count < argCount)
      {
        args.Add(string.Empty);
      }
      return args;
    }
  }
}