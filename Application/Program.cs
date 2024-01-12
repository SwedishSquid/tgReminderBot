
using Telegram.Bot;
using TelegramBotFirst;
using Domain;

namespace Application;

class Program
{

    public static void Main()
    {
        new MainBot(new TelegramBotClient(Secret.GetToken()), new IMessageHandler[0]).Run(true);
    }
}
