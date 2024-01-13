
using Telegram.Bot;
using TelegramBotFirst;
using Domain;

namespace Application;

class Program
{

    public static void Main()
    {
        new MainBot(new TelegramBotClient(Secret.GetToken()), new List<IMessageHandler>() { new ReminderMessageHandler(), new DefaultMessageHandler() }).Run(true);
    }
}
