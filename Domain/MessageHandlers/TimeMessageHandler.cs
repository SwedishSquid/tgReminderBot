using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Domain;

[MessageHandlerHelp("time", "use this command to configure your time zone", nameof(TimeMessageHandler.GetDetailedHelpMessage))]
public class TimeMessageHandler : IMessageHandler
{
    public async Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args)
    {
        if (!TryParseInput(args.Message.Text, out var parts))
            return false;

        _ = await TryProcessWithoutArgumentsAsync(parts, args)
            || await TryProcessSetUtcOffsetAsync(parts, args)
            || await ProcessDefaultAsync(parts, args);

        return true;
    }

    private bool TryParseInput(string? input, out string[] parts)
    {
        if (input is null)
        {
            parts = null;
            return false;
        }

        parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0 || parts[0].ToLower() != "/time")
            return false;

        return true;
    }

    private async Task<bool> TryProcessWithoutArgumentsAsync(string[] parts, IMessageHandlerArguments args)
    {
        if (parts.Length != 1)
        {
            return false;
        }
        var chat = args.Message.Chat;

        var utcTime = DateTime.UtcNow;
        var clientTime = (await args.StorageHandler.GetChatDataAsync(chat.Id)).ConvertUtcToLocal(utcTime);

        var reply = $"UTC time: {utcTime}\n\r" +
            $"Your time: {clientTime}\n\r" +
            $"Use \"/time setUtcOffset <data>\" to change your time settings" +
            $"Where <data> is \"utc<- or +><hours>\" \n\r" +
            $"Where <- or +> is one of \"-+\" and where <hours> is integer from 0 to 12\n\r" +
            $"Example: \"/time setUtcOffset utc+5\"";
        await SendText(args, reply);
        return true;
    }

    private async Task<bool> TryProcessSetUtcOffsetAsync(string[] parts, IMessageHandlerArguments args)
    {
        if (parts.Length < 2 || parts[1].ToLower() != "setUtcOffset".ToLower())
        {
            return false;
        }

        var chat = args.Message.Chat;

        if (parts.Length == 2)
        {
            await SendText(args, "wrong formatting: /time SetUtcOffset <arguments should be here>");
            return true;
        }

        if (TryParseUtcArgument(parts[2], out var offsetHours))
        {
            if (offsetHours > 12 || offsetHours < -12)
            {
                await SendText(args, $"offset too big");
            }
            else
            {
                var chatData = await args.StorageHandler.GetChatDataAsync(chat.Id);
                chatData.UtcOffset = TimeSpan.FromHours(offsetHours);
                await args.StorageHandler.SetChatDataAsync(chatData);

                var reply = "time zone changed; check by calling \"/time\"";
                await SendText(args, reply);
            }
        }
        else
        {
            var reply = $"cant parse UtcArgument {parts[2]}\n\r" +
                $"Example: utc-12";
            await SendText(args, reply);
        }
        return true;
    }

    private async Task<bool> ProcessDefaultAsync(string[] parts, IMessageHandlerArguments args)
    {
        var reply = $"unrecognized argument \"{parts[1]}\" of \"/time\" command";
        await SendText(args, reply);
        return true;
    }

    private async Task SendText(IMessageHandlerArguments args, string text)
    {
        var chat = args.Message.Chat;
        await args.BotClient.SendTextMessageAsync(chat, text);
    }

    private bool TryParseUtcArgument(string input, out int offsetHours)
    {
        var pattern = @"utc([-+]?\d+)";
        var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
        if (match.Success && int.TryParse(match.Groups[1].Value, out offsetHours))
        {
            return true;
        }
        offsetHours = 0;
        return false;
    }

    public static string GetDetailedHelpMessage()
    {
        return "usage: \"/time\" - get server time and your time; use to check your time zone \n\r" +
            "\"/time setUtcOffset utc<n>\" where n is integer with sign - sets your time zone\n\r" +
            "Example: \"/time setUtcOffset utc+5\"";
    }
}
