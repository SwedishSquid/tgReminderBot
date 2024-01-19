using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Domain;

public class TimeMessageHandler : IMessageHandler
{
    public async Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args)
    {
        var text = args.Message.Text;

        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0 || parts[0].ToLower() != "/time")
        {
            return false;
        }

        var chat = args.Message.Chat;

        if (parts.Length == 1)
        {
            var utcTime = DateTime.UtcNow;
            var clientTime = utcTime + (await args.StorageHandler.GetChatDataAsync(chat.Id)).UtcOffset;
            var textToSend = $"UTC time: {utcTime}\n\r" +
                $"Your time: {clientTime}\n\r" +
                $"Use \"/time setUtcOffset <data>\" to change your time settings" +
                $"Where <data> is \"utc<- or +><hours>\" " +
                $"Where <- or +> is one of \"-+\" and where <hours> is integer from 0 to 12";
            await args.BotClient.SendTextMessageAsync(chat, textToSend);
            return true;
        }

        if (parts[1].ToLower() == "setUtcOffset".ToLower())
        {
            if (parts.Length <= 2)
            {
                await args.BotClient.SendTextMessageAsync(chat, "wrong formatting: /time SetUtcOffset <arguments should be here>");
                return true;
            }
            var argument = parts[2];
            if (TryParseUtcArgument(parts[2], out var offsetHours))
            {
                if (offsetHours > 12 || offsetHours < -12)
                {
                    await args.BotClient.SendTextMessageAsync(chat, $"offset too big");
                }
                else
                {
                    var chatData = await args.StorageHandler.GetChatDataAsync(chat.Id);
                    chatData.UtcOffset = TimeSpan.FromHours(offsetHours);
                    await args.StorageHandler.SetChatDataAsync(chatData);
                    await args.BotClient.SendTextMessageAsync(chat, "time zone changed; check by calling \"/time\"");
                }
                return true;
            }
        }

        await args.BotClient.SendTextMessageAsync(chat, $"unrecognized argument \"{parts[1]}\" of \"/time\" command");
        return false;
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
}
