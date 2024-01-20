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

    }

    public async Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args)
    {
        throw new NotImplementedException();
    }
}
