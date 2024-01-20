using Domain;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using System;
using System.Linq;
using System.Text;
using Telegram.Bot;
using TelegramBotFirst;

namespace Application;

internal class AppModule : NinjectModule
{
    public override void Load()
    {
        Kernel.Bind<ITelegramBotClient>().ToConstant(new TelegramBotClient(Secret.GetToken())).InSingletonScope();

        Kernel.Bind(
            c => c
            .FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<IMessageHandler>()
            .BindAllInterfaces());

        Kernel.Bind<IReminderMessageParser>()
            .To<ReminderMessageParserList>()
            .WhenInjectedInto<ReminderMessageHandler>();
    }
}
