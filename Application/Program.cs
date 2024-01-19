
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
        //var container = ConfigureContainer();
        //container.Get<IBot>().Run(true);
        EasyForMeComposition().Run(true);
    }

    private static StandardKernel ConfigureContainer()
    {
        return new StandardKernel(new NinjectModule[] {new AppModule(), new DomainModule()});
    }

    private static IBot EasyForMeComposition()
    {
        var messageHandlers = new List<IMessageHandler>() {
            new ReminderMessageHandler(new ReminderMessageParser()),
            new TimeMessageHandler(),
            new DefaultMessageHandler(),
        };
        return new MainBot(new TelegramBotClient(Secret.GetToken()), messageHandlers, new SimpleStorageHandler());
    }
}
