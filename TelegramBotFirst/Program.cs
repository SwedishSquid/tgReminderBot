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

class Test
{
    public static void Tes()
    {
        var d = new Dictionary<string, string>
        {
            { "StatusCode", "200" },
            { "body", "Hi" }
        };
        var s = JsonSerializer.Serialize(d);
        Console.WriteLine(s);
    }
}


public class Program
{

    private static ITelegramBotClient bot = new TelegramBotClient(Secret.GetToken());

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient,
        Update update, CancellationToken cancellationToken)
    {
        //Console.WriteLine(JsonSerializer.Serialize(update));
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            if (message.Text.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat, "welcome aboard");
                return;
            }

            if (message.Text.Split(' ')[0].ToLower() == "/echo")
            {
                var textToSend = message.Text.Substring(5);
                if (textToSend == "")
                {
                    textToSend = "empty";
                }
                await botClient.SendTextMessageAsync(message.Chat, textToSend);
                return;
            }

            await botClient.SendTextMessageAsync(message.Chat, "hi hello. just regular response");
        }
    }

    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine("exception recieved");
        Console.WriteLine(exception.Message);
    }

    public static void Run(bool forever = false)
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var recieverOptions = new ReceiverOptions()
        {
            AllowedUpdates = { }, //all update types
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            recieverOptions,
            cancellationToken
        );
        if ( forever )
        {
            Console.ReadLine();
        }
        else
        {
            Thread.Sleep(1000);
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine($"bot {bot.GetMeAsync().Result.FirstName} activated");

        Run(forever:true);
    }
}