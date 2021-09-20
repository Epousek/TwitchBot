﻿using System;
using System.Threading.Tasks;
using Npgsql;

namespace TwitchBot.src
{
  class Program
  {
    static void Main(string[] args)
    {
      Config.SetConfig();
      Console.WriteLine(Config.Credentials.Username);
      Task.Run(Authentication.StartRefreshingTokens);

      Bot bot = new("epousek");
      Console.ReadLine();
    }
  }
}
