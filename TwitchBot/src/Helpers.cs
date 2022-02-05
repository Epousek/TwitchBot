namespace TwitchBot
{
  public static class Helpers
  {
    public static string FirstToUpper(string text)
    {
      return char.ToUpper(text[0]) + text[1..];
    }
  }
}
