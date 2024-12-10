using BaseballScoringApp.Models;
using CommunityToolkit.Maui;
using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using System.Runtime.CompilerServices;

namespace BaseballScoringApp
{
    public static class MauiProgram
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                // this tells maui what is the application class it should use to start, App
                .UseMauiApp<App>()

                // Initialize the .NET MAUI Community Toolkit by adding the below line of code
                .UseMauiCommunityToolkit()

                // After initializing the .NET MAUI Community Toolkit, optionally add additional fonts
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Logging.AddTraceLogger(_ => { });
            builder.Logging.AddInMemoryLogger(_ => { });
            builder.Logging.AddStreamingFileLogger(options =>
            {
                options.RetainDays = 2;
                //writes to C:\Users\robin\AppData\Local\User Name\com.companyname.baseballscoringapp\Cache\LoggingApp folder on laptop
                options.FolderPath = Path.Combine(FileSystem.CacheDirectory, "LoggingApp"); 
            });
            //builder.Services.AddTransient<MainPageLogin>();
            builder.Services.AddTransient<App>(); //Dependency injection, see consturctor App added logger
            
            // Register audio service from plugin.maui.audio
            builder.Services.AddSingleton<IAudioManager, AudioManager>();
            // Register ScoringContentPage for dependency injection (passing audiomanager in constructor)...
            builder.Services.AddTransient<ScoringContentPage>();

            var app = builder.Build();

            // Store the service provider so that it can be called later (for getting audiomanager from service)
            ServiceProvider = app.Services;


            return app;
        }
    }
}
