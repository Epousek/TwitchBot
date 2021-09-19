using System;
using System.Threading.Tasks;

namespace TwitchBot.src
{
  class Program
  {
    static void Main(string[] args)
    {
      Config.SetConfig();
      Console.WriteLine(Config.Credentials.Username);
      Task.Run(Authentication.StartRefreshingTokens);

      Console.Read();
    }
  }
}
