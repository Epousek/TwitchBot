using System;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Humanizer.Localisation;
using TwitchBot.Models;
using TwitchBot.Connections;

namespace TwitchBot.Commands.Status
{
  public static class GetSetStatus
  {
    public static async Task CheckStatus(ChatMessageModel message)
    {
      var status = await DatabaseConnections.GetUserStatus(message.Channel, message.Username).ConfigureAwait(false);
      if (status == null)
        return;
      if (status.Status == Enums.Status.None)
        return;
      if (message.Message.StartsWith(Bot.Prefix + status.Status.ToString().ToLower()))
        return; //TODO: update status

      //TODO: changing from one status to another
      // Enums.Status foundStatus;
      // foreach (var s in Enum.GetNames(typeof(Enums.Status)))
      // {
      //   if (message.Message.StartsWith(Bot.Prefix + s))
      //   {
      //     foundStatus = Enum.Parse<Enums.Status>(s);
      //     return;
      //   }
      // }
      
      var builder = new StringBuilder();
      builder.Append(message.Username)
        .Append(' ');
      switch (status.Status)
      {
        case Enums.Status.Afk: 
          builder.Append("už není afk");
          if (string.IsNullOrEmpty(status.Message))
            builder.Append($"! ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          else
            builder.Append($": {status.Message} ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          break;
        case Enums.Status.Gn:
          builder.Append("se právě probudil(a)");
          if (string.IsNullOrEmpty(status.Message))
            builder.Append($"! ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          else
            builder.Append($": {status.Message} ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          break;
        case Enums.Status.Food:
          builder.Append("právě dojedl(a)");
          if (string.IsNullOrEmpty(status.Message))
            builder.Append($"! ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          else
            builder.Append($": {status.Message} ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          break;
        case Enums.Status.Work:
          builder.Append("právě dopracoval");
          if (string.IsNullOrEmpty(status.Message))
            builder.Append($"! ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          else
            builder.Append($": {status.Message} ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          break;
        case Enums.Status.School:
          builder.Append("už není ve škole");
          if (string.IsNullOrEmpty(status.Message))
            builder.Append($"! ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          else
            builder.Append($": {status.Message} ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          break;
      }

        Bot.WriteMessage(builder.ToString(), message.Channel);
        status.Status = Enums.Status.None;
        await DatabaseConnections.UpdateUserStatus(status).ConfigureAwait(false);
    }
  }
}

