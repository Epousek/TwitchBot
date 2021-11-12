using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.src.Models
{
  public interface ICommand
  {
    string Name { get; }
    string About { get; }
    string Help { get; }
    int Permission { get; }
    bool OfflineOnly { get; }
    bool UsableByBanned { get; }
    bool Optoutable { get; }
    int TimesUsedSinceRestart { get; }
    int? TimesUsedTotal { get; set; }

    string GetAbout();
    string GetHelp();
  }
}
