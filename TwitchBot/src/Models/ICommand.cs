﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.src.Enums;

namespace TwitchBot.src.Models
{
  public interface ICommand
  {
    string Name { get; }
    string About { get; }
    string Help { get; }
    Permission Permission { get; }
    bool OfflineOnly { get; }
    bool UsableByBanned { get; }
    bool Optoutable { get; }
    int TimesUsedSinceRestart { get; set; }
    int? TimesUsedTotal { get; set; }

    public Task UseCommandAsync(ChatMessageModel message);
  }
}
