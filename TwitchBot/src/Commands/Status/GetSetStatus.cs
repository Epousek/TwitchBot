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
    public static string StatusChange(string username, Enums.Status from, Enums.Status to)
      => $"{username} změnil(a) svůj status z {from.ToString().ToLower()} na {to.ToString().ToLower()}";

    public static StatusModel CreateStatus(ChatMessageModel message, Enums.Status status)
    {
      var comArgs = new CommandArguments(message);
      var args = comArgs.GetOneArgument();

      return new StatusModel
      {
        Channel = message.Channel,
        Username = message.Username,
        StatusSince = DateTime.Now,
        CurrentStatus = status,
        Message = args.Count == 0 ? "" : args[0]
      };
    }

    public static async Task CheckStatus(ChatMessageModel message)
    {
      var status = await DatabaseConnections.GetUserStatus(message.Channel, message.Username).ConfigureAwait(false);
      if (status == null)
        return;
      if (status.CurrentStatus == Enums.Status.None)
        return;
      if (Enum.GetNames(typeof(Enums.Status)).Any(x => message.Message.StartsWith(Bot.Prefix + x.ToLower())))
        return; //TODO: update status
      if (string.Equals(message.Message, Bot.Prefix + "rafk") || string.Equals(message.Message, Bot.Prefix + "cafk"))
        return; //TODO: check dynamically

      var builder = new StringBuilder();
      builder
        .Append(message.Username)
        .Append(' ');
      switch (status.CurrentStatus)
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
          builder.Append("se už určitě naučil vše co potřeboval");
          if (string.IsNullOrEmpty(status.Message))
            builder.Append($"! ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          else
            builder.Append($": {status.Message} ({(DateTime.Now - status.StatusSince).Humanize(3, minUnit: TimeUnit.Second, culture: new CultureInfo("cs-CS"))})");
          break;
      }

      Bot.WriteMessage(builder.ToString(), message.Channel);
      status.CurrentStatus = Enums.Status.None;
      await DatabaseConnections.UpdateUserStatus(status).ConfigureAwait(false);
    }
  }
}

