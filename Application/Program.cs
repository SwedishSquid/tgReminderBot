
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
                        container.Get<DefaultMessageHandler>(),
                    },
                    container.Get<IReminderDataStorage>()))
            .InSingletonScope();

        container.Bind<IBot>().ToConstant(container.Get<MainBot>());
        return container;
    }
}
