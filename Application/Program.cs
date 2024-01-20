
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
        var maxRemindersCountSendPerSecond = 5;

        var relPathToSomeFolderInProject = "../../../../Infrastructure/AppData/persistence";
        var pathToPersistenceFolder = relPathToSomeFolderInProject;     //can be any path to a folder;
                                                                        //bot will store data there to resume if restarted

        var container = new StandardKernel(new NinjectModule[] {new AppModule(), new DomainModule()});

        container.Bind<IStorageHandler>().ToConstant(new StorageHandler(pathToPersistenceFolder));

        container.Bind<MainBot>()
            .ToConstant(
                new MainBot(
                    container.Get<ITelegramBotClient>(),
                    new List<IMessageHandler>() {
                        container.Get<StartMessageHandler>(),
                        container.Get<HelpMessageHandler>(),
                        container.Get<ReminderMessageHandler>(),
                        container.Get<TimeMessageHandler>(),
                        container.Get<DefaultMessageHandler>(),
                    },
                    container.Get<IStorageHandler>(),
                    maxRemindersCountSendPerSecond)
                )
            .InSingletonScope();

        container.Bind<IBot>().ToConstant(container.Get<MainBot>());
        return container;
    }
}
