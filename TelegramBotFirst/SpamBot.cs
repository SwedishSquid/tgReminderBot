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

internal class SpamBot
{
    private static ITelegramBotClient bot = new TelegramBotClient(Secret.GetToken());

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient,
        Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;

            Console.WriteLine($"id: {message.Chat.Id}  type: {message.Chat.Type}");

            await botClient.SendTextMessageAsync(message.Chat, "hi hello. just regular response");
        }
    }

    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine("exception recieved");
        Console.WriteLine(exception.Message);
    }

    private static async Task SendMessageByMyself()
    {
        var chat = new Chat();
        /*chat.Id = <Insert Your chat Id>;
        chat.Type = ChatType.Private;*/
        await bot.SendTextMessageAsync(chat, "message that I send by myself");
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

        SendMessageByMyself().Wait();
    }
}

