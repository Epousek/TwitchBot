using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeepL;
using Serilog;
using TwitchBot.Connections;
using TwitchBot.Enums;
using TwitchBot.Interfaces;
using TwitchBot.Models;

namespace TwitchBot.Commands;

public class Translate : ICommand
{
  public string Name { get; } = nameof(Translate);
  public string AboutCommand { get; } = "Přelož text, max 300 znaků, pomocí DeepL API.";
  public string HelpMessage { get; } = "$translate *jazyk do kterého se má text přeložit (např. cz, en, es, fr...)* \"text\"";
  public string[] Aliases { get; } = { "přelož" };
  public Permission Permission { get; } = Permission.Regular;
  public TimeSpan Cooldown { get; } = TimeSpan.FromSeconds(1);
  public DateTime LastUsed { get; set; }
  public bool OfflineOnly { get; } = false;
  public bool UsableByBanned { get; } = false;
  public bool Optoutable { get; } = false;
  public int TimesUsedSinceRestart { get; set; }
  public int? TimesUsedTotal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


  public async Task UseCommandAsync(ChatMessageModel message)
  {
    var comArgs = new CommandArguments(message);
    var args = comArgs.GetXArguments(2);
    args[1] = args[1].Replace("\"", "");
    args[0] = string.Equals(args[0], "en", StringComparison.OrdinalIgnoreCase) ? "EN-US" : args[0];
    
    if (args[0].StartsWith('"'))
    {
      Bot.WriteMessage($"@{message.Username} zadej prosím jazyk do kterého chceš text přeložit (jazyky: https://pastebin.com/25PUFnKu )", message.Channel);
    }
    else if (string.IsNullOrEmpty(args[1]))
    {
      Bot.WriteMessage($"@{message.Username} zadej prosím text který chceš přeložit.", message.Channel);
    }
    else
    {
      var translator = new DeeplClient();
      try
      {
        var result = await translator.TranslateDetectLanguageAsync(args[1], args[0]).ConfigureAwait(false);
        Bot.WriteMessage($"@{message.Username} z {result.LangFrom} do {result.LangTo}: \"{result.Text}\"", message.Channel);
      }
      catch (ArgumentException e)
      {
        Bot.WriteMessage($"@{message.Username} jazyk nebyl nalezen - špatný kód/nejde do něj překládat. (jazyky: https://pastebin.com/25PUFnKu )", message.Channel);
      }
      catch (DeepLException e)
      {
        if (e.Message.Contains("target_lang"))
        {
          Bot.WriteMessage($"@{message.Username} jazyk nebyl nalezen - špatný kód/nejde do něj překládat. (jazyky: https://pastebin.com/25PUFnKu )", message.Channel);
        }
        else
        {
          Bot.WriteMessage($"@{message.Username} chyba při komunikaci s DeepL API. :/ (@epousek)", message.Channel);
          Log.Error("Chyba při komunikaci s DeepL API: {e}", e);
        }
      }
    }
  }
}