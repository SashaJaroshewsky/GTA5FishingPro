
using FishingBot.App.Core;
using FishingBot.App.Services.Implementation;
using FishingBot.App.Services.Interfaces;
using GlobalHotKey;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;



namespace FishingBot.App
{
    internal class Program
    {
        private static CancellationTokenSource _cts;
        private static Task _fishingTask;
        private static Core.FishingBot _fishingBot;

        static async Task Main()
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            var services = ConfigureServices();
            var fishingBot = services.GetRequiredService<Core.FishingBot>();
            _fishingBot = fishingBot;

            var hotkeyThread = new Thread(RegisterGlobalHotkeys);
            hotkeyThread.SetApartmentState(ApartmentState.STA);
            hotkeyThread.Name = "HotkeyThread";
            hotkeyThread.Start();

            // Основний потік очікує, поки всі фонові завдання завершаться
            await Task.Delay(Timeout.Infinite);
            // 
            //Application.Run();
        }

        private static void RegisterGlobalHotkeys()
        {
            var hotKeyManager = new HotKeyManager();
            hotKeyManager.Register(Key.F9, ModifierKeys.Control); // Ctrl+F9 для зупинки
            hotKeyManager.Register(Key.F10, ModifierKeys.Control); // Ctrl+F10 для запуску

            hotKeyManager.KeyPressed += OnHotKeyPressedStopBot;

            Console.WriteLine("Натисніть Ctrl+F9 для запуску.");
            Console.WriteLine("Натисніть Ctrl+F10 для зупинки.");
            Application.Run();
        }

        private static void OnHotKeyPressedStopBot(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Modifiers == ModifierKeys.Control && e.HotKey.Key == Key.F9)
                StartFishing();
            if (e.HotKey.Modifiers == ModifierKeys.Control && e.HotKey.Key == Key.F10)
                StopFishing();
        }

        private static void OnHotKeyPressedStartBot(object sender, KeyPressedEventArgs e)
        {
            StartFishing();
        }

        private static void StartFishing()
        {
            if (_fishingTask != null && !_fishingTask.IsCompleted)
            {
                Console.WriteLine("Риболовний цикл вже запущено.");
                return;
            }

            Console.WriteLine("Запуск риболовного циклу.");
            _cts = new CancellationTokenSource();
            _fishingTask = Task.Run(() => _fishingBot.StartFishingLoop(_cts.Token));
        }

        private static void StopFishing()
        {
            if (_cts == null || _cts.IsCancellationRequested)
            {
                Console.WriteLine("Риболовний цикл вже зупинено.");
                return;
            }

            Console.WriteLine("Зупинка риболовного циклу.");
            _cts.Cancel();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IAppConfigurationService, AppConfigurationService>();

            services.AddSingleton<IScreenCaptureService, ScreenCaptureService>();
            services.AddSingleton<ITemplateMatchingService, TemplateMatchingService>();
            services.AddSingleton<IInputSimulationService, InputSimulationService>();
            services.AddSingleton<IImageComparatorService, ImageComparatorService>();


            services.AddSingleton<Core.FishingBot>();
            services.AddSingleton<FishTracker>();
            services.AddSingleton<HookController>();
            services.AddSingleton<MessageTracker>();

            return services.BuildServiceProvider();
        }
    }
}
