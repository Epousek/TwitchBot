using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.src
{
  public static class Helpers
  {
    public static string FirstToUpper(string text)
    {
      return char.ToUpper(text[0]) + text[1..];
    }
  }
}
