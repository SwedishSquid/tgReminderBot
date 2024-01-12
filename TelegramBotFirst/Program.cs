using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using System.Text.Json;
using Telegram.Bot.Types.Enums;


namespace TelegramBotFirst;


public class Program
{
    public static void Main(string[] args)
    {
        SpamBot.Run();
    }
}

