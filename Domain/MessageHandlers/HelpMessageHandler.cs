using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Domain;

public class HelpMessageHandler : IMessageHandler
{
    private static readonly string helpMessage;
    private static readonly Dictionary<string, string?> messageHandlersDetailedHelpByName;
    private static readonly Regex matchRe = new (@"^/help(?:\s+(.+))?");

    static HelpMessageHandler()
    {
        var messageHandlerTypesWithDescription = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(IMessageHandler).IsAssignableFrom(type)
                        && !type.IsInterface
                        && type.GetCustomAttribute<MessageHandlerHelpAttribute>() is not null)
            .ToList();

        helpMessage = CreateHelpMessage(
            messageHandlerTypesWithDescription.Select(type => type.GetCustomAttribute<MessageHandlerHelpAttribute>()));

        messageHandlersDetailedHelpByName = messageHandlerTypesWithDescription.ToDictionary(
            messageHandlerType => messageHandlerType.GetCustomAttribute<MessageHandlerHelpAttribute>().Name,
            GetHandlerMessageDetailedHelpFromType);
    }

    private static string CreateHelpMessage(IEnumerable<MessageHandlerHelpAttribute> messageHandlersDescription)
    {
        var helpMessageBuilder = new StringBuilder();
        foreach(var messageHandlerDescription in messageHandlersDescription)
        {
            helpMessageBuilder.AppendLine($"{messageHandlerDescription.Name} - {messageHandlerDescription.Description}");
        }
        helpMessageBuilder.AppendLine($"Use \"/help <command name>\" to know deatiled info");
        return helpMessageBuilder.ToString();
    }

    private static string? GetHandlerMessageDetailedHelpFromType(Type messageHandlerType)
    {
        var getDetailedHelpMethod = messageHandlerType.GetMethod(nameof(IMessageHandler.GetDetailedHelp));
        return (string)getDetailedHelpMethod?.Invoke(null, null);
    }

    public async Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args)
    {
        if (args.Message.Text is null)
            return false;
        var matchObj = matchRe.Match(args.Message.Text);
        if (!matchObj.Success)
            return false;
        var handlerName = matchObj.Groups[1].Value;
        if (handlerName == "")
        {
            await SendHelpMessageAsync(args.Message.Chat, args.BotClient);
            return true;
        }
        else if (messageHandlersDetailedHelpByName.TryGetValue(handlerName, out var detailedHelp))
        {
            if (detailedHelp is null)
                await SendMessageDetailedHelpMissing(args.Message.Chat, args.BotClient);
            else
                await SendDetailedHelpMessageAsync(args.Message.Chat, args.BotClient, detailedHelp);
            return true;
        }
        return false;
    }

    private async Task SendHelpMessageAsync(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, helpMessage);
    }

    private async Task SendDetailedHelpMessageAsync(Chat chat, ITelegramBotClient botClient, string detailedHelpMessage)
    {
        await botClient.SendTextMessageAsync(chat, detailedHelpMessage);
    }

    private async Task SendMessageDetailedHelpMissing(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, "detailed help missing");
    }
}
