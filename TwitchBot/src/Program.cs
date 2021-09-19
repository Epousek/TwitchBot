using System;

namespace TwitchBot.src
{
  class Program
  {
    static void Main(string[] args)
    {
      Config config = new();
      Console.WriteLine(config.ToString());
    }
  }
}
