using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application;

[MessageHandlerHelp("help", "sends detailed help on the command", nameof(GetDetailedHelp))]
public class HelpMessageHandler : IMessageHandler
{
    private static readonly string helpMessage;
    private static readonly Dictionary<string, string?> messageHandlersDetailedHelpByName;
    private static readonly Regex matchRe = new (@"^/help(?:\s+(.+))?");

    static HelpMessageHandler()
    {
        var messageHandlerTypesWithHelp = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(IMessageHandler).IsAssignableFrom(type) && !type.IsInterface)
            .Select(messageHandlerType => (
                MessageHandlerType: messageHandlerType,
                Help: messageHandlerType.GetCustomAttribute<MessageHandlerHelpAttribute>()
                ))
            .Where(messageHandler => messageHandler.Help is not null)
            .ToList();

        helpMessage = CreateHelpMessage(messageHandlerTypesWithHelp.Select(messageHandler => messageHandler.Help));

        messageHandlersDetailedHelpByName = messageHandlerTypesWithHelp.ToDictionary(
            messageHandler => messageHandler.Help.Name,
            messageHandler => FindHandlerMessageDetailedHelpFromType(
                messageHandler.MessageHandlerType, 
                messageHandler.Help.GetDetailedHelpMethodName));
    }

    private static string CreateHelpMessage(IEnumerable<MessageHandlerHelpAttribute> messageHandlersDescription)
    {
        var helpMessageBuilder = new StringBuilder();
        helpMessageBuilder.AppendLine("<command name> - <description>");
        foreach(var messageHandlerDescription in messageHandlersDescription)
        {
            helpMessageBuilder.AppendLine($"{messageHandlerDescription.Name} - {messageHandlerDescription.Description}");
        }
        helpMessageBuilder.AppendLine($"Use \"/help <command name>\" to know deatiled info");
        return helpMessageBuilder.ToString();
    }

    private static string? FindHandlerMessageDetailedHelpFromType(
        Type messageHandlerType, string getDetailedHelpMethodName)
    {
        var getDetailedHelpMethod = messageHandlerType.GetMethod(getDetailedHelpMethodName);
        return (string?)getDetailedHelpMethod?.Invoke(null, null);
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

    private static async Task SendHelpMessageAsync(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, helpMessage);
    }

    private static async Task SendDetailedHelpMessageAsync(Chat chat, ITelegramBotClient botClient, string detailedHelpMessage)
    {
        await botClient.SendTextMessageAsync(chat, detailedHelpMessage);
    }

    private static async Task SendMessageDetailedHelpMissing(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, "detailed help missing");
    }

    public static string GetDetailedHelp()
    {
        return "\"/help <command name>\" (sends detailed help on the <command name>)";
    }
}
