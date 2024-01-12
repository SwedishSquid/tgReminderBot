using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBotFirst;


public class ExampleBot
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
        Console.WriteLine($"bot {bot.GetMeAsync().Result.FirstName} activated");

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
        if (forever)
        {
            Console.ReadLine();
        }
        else
        {
            Thread.Sleep(1000);
        }
    }
}

