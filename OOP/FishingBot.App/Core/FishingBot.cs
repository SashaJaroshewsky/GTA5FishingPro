using FishingBot.App.Configuration;
using FishingBot.App.Services.Interfaces;
using System;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace FishingBot.App.Core
{
    internal class FishingBot
    {
        private readonly IInputSimulationService _inputSimulation;
        private readonly AppConfiguration _config;
        private readonly FishTracker _fishTracker;
        private readonly HookController _hookController;
        private readonly MessageTracker _messageTracker;

        public FishingBot(
            IInputSimulationService inputSimulation,
            AppConfiguration config,
            MessageTracker messageTracker,
            FishTracker fishTracker,
            HookController hookController)
        {
            _inputSimulation = inputSimulation;
            _config = config;
            _messageTracker = messageTracker;
            _fishTracker = fishTracker;
            _hookController = hookController;
        }

        public async Task StartFishingLoop()
        {
            while (true)
            {
                try
                {
                    await StartFishing();
                    await WaitForCatchNotification();
                    await WaitForFish();
                    await StartMiniGame();
                    await Task.Delay(2000);
                    await FollowFish();
                    await WaitForCatchNotification();
                    await Task.Delay(400);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка під час риболовлі: {ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }

        private async Task StartFishing()
        {
            Console.WriteLine("Запуск риболовлі...");
            await _inputSimulation.PressKey(VirtualKeyCode.VK_1);
            await Task.Delay(1000);
        }




        private async Task WaitForFish()
        {
            Console.WriteLine("Чекаємо на рибу...");
            await _messageTracker.TrackMessage(_config.FishAlertRegion, _config.FishAlertTemplate, _config.TemplateMatchThreshold);
            Console.WriteLine("Вона поблизу!");

        }

        private async Task<bool> CheckingMiniGameActivity()
        {
            if (await _messageTracker.TrackMessage(_config.MiniGameSearchRegion, _config.MiniGameImageTemplate, _config.MiniGameNotificationThreshold))
            {
                Console.WriteLine("Міні гра відкрита");
                return true;
            }
            else
            {
                Console.WriteLine("Міні гра закрита");
                return false;
            }
        }

        private async Task StartMiniGame()
        {
            Console.WriteLine("Запуск міні-гри...");
            await _inputSimulation.PressKey(VirtualKeyCode.VK_E);
            await Task.Delay(200);
        }

        private async Task FollowFish()
        {
            Console.WriteLine("Де ж вона пливе...");
           
            while (true)
            {
                if (!await CheckingMiniGameActivity())
                {
                    return;
                }

                int fishPosition = await _fishTracker.FishTracking();
                Console.WriteLine("Ось вона. Починаю рибалити");
                await _hookController.ControlHook(fishPosition);
                break;
            }
        }

        private async Task WaitForCatchNotification()
        {
            Console.WriteLine("Чекаємо на підтвердження...");
            await _messageTracker.TrackMessage(_config.CatchNotificationRegion, _config.CatchNotificationTemplate, _config.CatchNotificationThreshold);
            Console.WriteLine("Повідомлення зенайденно!");
        }
    }
}
