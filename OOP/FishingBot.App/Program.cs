using FishingBot.App.Configuration;
using FishingBot.App.Services.Implementation;
using FishingBot.App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingBot.App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new AppConfiguration();
            var services = ConfigureServices(config);
            var fishingBot = new Core.FishingBot(
                services.GetRequiredService<IScreenCaptureService>(),
                services.GetRequiredService<ITemplateMatchingService>(),
                services.GetRequiredService<IInputSimulationService>(),
                config
            );

            Console.WriteLine("Починається процес авто рибо ловлі");
            await Task.Delay(5000);

            await Task.WhenAny(
                fishingBot.StartFishingLoop(),
                MonitorExitKey()
            );
        }

        private static IServiceProvider ConfigureServices(AppConfiguration config)
        {
            var services = new ServiceCollection();

            services.AddSingleton(config);
            services.AddSingleton<IScreenCaptureService, ScreenCaptureService>();
            services.AddSingleton<ITemplateMatchingService, TemplateMatchingService>();
            services.AddSingleton<IInputSimulationService, InputSimulationService>();

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
