using FishingBot.App.Configuration;
using FishingBot.App.Core;
using FishingBot.App.Services.Implementation;
using FishingBot.App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace FishingBot.App
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            var services = ConfigureServices();

            var fishingBot = services.GetRequiredService<Core.FishingBot>();

            await Task.WhenAny(
                fishingBot.StartFishingLoop(),
                MonitorExitKey()
            );
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IScreenCaptureService, ScreenCaptureService>();
            services.AddSingleton<ITemplateMatchingService, TemplateMatchingService>();
            services.AddSingleton<IInputSimulationService, InputSimulationService>();
            services.AddSingleton<IImageComparatorService, ImageComparatorService>();

            services.AddSingleton<AppConfiguration>();
            services.AddSingleton<Core.FishingBot>();
            services.AddSingleton<FishTracker>();
            services.AddSingleton<HookController>();
            services.AddSingleton<MessageTracker>();

            return services.BuildServiceProvider();
        }

        private static async Task MonitorExitKey()
        {
            Console.WriteLine("Натисніть 'Q' для виходу.");
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
                await Task.Delay(200);
            }
        }
    }
}
