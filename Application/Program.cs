
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
        var container = ConfigureContainer();
        container.Get<IBot>().Run(true);
    }

    private static StandardKernel ConfigureContainer()
    {
        var container = new StandardKernel(new NinjectModule[] {new AppModule(), new DomainModule()});

        container.Bind<MainBot>()
            .ToConstant(
                new MainBot(
                    container.Get<ITelegramBotClient>(),
                    new List<IMessageHandler>() {
                        container.Get<StartMessageHandler>(),
                        container.Get<ReminderMessageHandler>(),
                        container.Get<TimeMessageHandler>(),
                        container.Get<DefaultMessageHandler>(),
                    },
                    container.Get<IStorageHandler>()))
            .InSingletonScope();

        container.Bind<IBot>().ToConstant(container.Get<MainBot>());
        return container;
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
