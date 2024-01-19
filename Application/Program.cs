
using Telegram.Bot;
using TelegramBotFirst;
using Domain;
using Ninject;
using Ninject.Modules;

namespace Application;

class Program
{
    public static void Main()
    {
        //new MainBot(new TelegramBotClient(Secret.GetToken()), new List<IMessageHandler>() { new ReminderMessageHandler(new ReminderMessageParser()), new DefaultMessageHandler() }, new InMemoryReminderDataStorage()).Run(true);
        var container = ConfigureContainer();
        container.Get<IBot>().Run(true);
    }

    private static StandardKernel ConfigureContainer()
    {
        return new StandardKernel(new NinjectModule[] {new AppModule(), new DomainModule()});
    }
}
