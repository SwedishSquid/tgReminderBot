using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using System.Text.Json;
using Telegram.Bot.Types.Enums;
using System.Text.RegularExpressions;


namespace TelegramBotFirst;


public class Program
{
    public static void Main(string[] args)
    {
        var regex = new Regex(@"utc[-+](\d+)");
        var match = regex.Match("utc-0");
        Console.WriteLine(int.Parse(match.Groups[1].Value));
    }
}

