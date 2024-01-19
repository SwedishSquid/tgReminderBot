using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBotFirst;

namespace Application;

internal class AppModule : NinjectModule
{
    public override void Load()
    {
        Kernel.Bind<IBot>().To<MainBot>().InSingletonScope();
        Kernel.Bind<ITelegramBotClient>().ToConstant(new TelegramBotClient(Secret.GetToken())).InSingletonScope();
    }
}
