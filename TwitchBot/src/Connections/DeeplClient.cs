using System.Threading.Tasks;
using DeepL;
using TwitchBot.Models;

namespace TwitchBot.Connections
{
  public  class DeeplClient
  {
    private readonly Translator _translator;

    public DeeplClient()
    {
      _translator = new Translator(SecretsConfig.Credentials.DeeplKey);
    }

    public async Task<Translation> TranslateWithLanguagesAsync(string translate, string langTo, string langFrom)
      => await Translate(translate, langTo, langFrom).ConfigureAwait(false);

    public async Task<Translation> TranslateDetectLanguageAsync(string translate, string langTo)
      => await Translate(translate, langTo).ConfigureAwait(false);

    private async Task<Translation> Translate(string translate, string langTo, string langFrom = null)
    {
      var text = await _translator.TranslateTextAsync(translate, langFrom, langTo);

      return new Translation()
      {
        LangFrom = langFrom ?? text.DetectedSourceLanguageCode,
        LangTo = langTo,
        Text = text.Text
      };
    }
  }
}

