using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain;

public class HelpMessageHandler : IMessageHandler
{
    private static readonly string helpMessage;
    private static readonly Dictionary<string, Type> IMessageHandlersByName;
    private static readonly Regex matchRe = new (@"^/help\s+(\w*)");

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
        IMessageHandlersByName = messageHandlerTypesWithDescription.ToDictionary(
            messageHandlerType => messageHandlerType.GetCustomAttribute<MessageHandlerHelpAttribute>().Name,
            messageHandlerType => messageHandlerType);
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

    public async Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args)
    {
        throw new NotImplementedException();
    }
}
