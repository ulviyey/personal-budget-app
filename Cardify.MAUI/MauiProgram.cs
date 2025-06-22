﻿
using Cardify.Core.Services;
using Cardify.MAUI.Services;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Cardify.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseSkiaSharp();
        ;

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<ILoginService, MockLoginService>();

        return builder.Build();
    }
}
