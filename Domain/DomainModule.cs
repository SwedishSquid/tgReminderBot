﻿using System;
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
        Kernel.Bind(
            c => c
            .FromThisAssembly()
            .SelectAllClasses()
            .InheritedFrom<IReminderMessageParser>()
            .Excluding<ReminderMessageParserList>()
            .BindAllInterfaces());

        //Kernel.Bind<IStorageHandler>().To<StorageHandler>().InSingletonScope();
    }
}
