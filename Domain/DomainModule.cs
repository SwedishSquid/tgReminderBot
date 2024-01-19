using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace Domain;

public class DomainModule : NinjectModule
{
    public override void Load()
    {
        //Kernel.Bind(
        //    c => c
        //    .FromThisAssembly()
        //    .SelectAllClasses()
        //    .InheritedFrom<IMessageHandler>()
        //    .BindAllInterfaces());
        Kernel.Bind<IMessageHandler>().To<ReminderMessageHandler>().InSingletonScope();
        //Kernel.Bind<IMessageHandler>().To<DefaultMessageHandler>().InSingletonScope();
        Kernel.Bind(
            c => c
            .FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<IReminderMessageParser>()
            .Excluding<ReminderMessageParserList>()
            .BindAllInterfaces());
        //Kernel.Bind<IReminderMessageParser>().To<ReminderMessageParser>().InSingletonScope();
        Kernel
            .Bind<IReminderMessageParser>()
            .To<ReminderMessageParserList>()
            .WhenInjectedInto<ReminderMessageHandler>();
        Kernel.Bind<IReminderDataStorage>().To<InMemoryReminderDataStorage>().InSingletonScope();
    }
}
