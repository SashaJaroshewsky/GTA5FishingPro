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
            Console.WriteLine("Починається процес автоматичної риболовлі");
            await Task.Delay(5000);

            while (true)
            {
                try
                {
                    await CastFishingRod();
                    //await WaitForCatchNotification();
                    WaitForFish();
                    await StartMiniGame();
                    await Task.Delay(1000);
                    await FollowFish();
                    WaitForCatchNotification();
                    await Task.Delay(400);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка під час риболовлі: {ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }

        private async Task CastFishingRod()
        {
            Console.WriteLine("Закидаю вудку...");
            _inputSimulation.PressKey(VirtualKeyCode.VK_1);
            await Task.Delay(1000);
        }




        private void WaitForFish()
        {
            Console.WriteLine("Чекаємо на рибу...");
            _messageTracker.TrackMessageCycle(_config.FishAlertRegion, _config.FishAlertTemplate, _config.TemplateMatchThreshold);
            Console.WriteLine("Вона поблизу!");

        }

        private async Task<bool> CheckingMiniGameActivity()
        {
            await Task.Delay(300);
            if (_messageTracker.TrackMessage(_config.MiniGameSearchRegion, _config.MiniGameImageTemplate, _config.MiniGameNotificationThreshold))
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
            _inputSimulation.PressKey(VirtualKeyCode.VK_E);
            await Task.Delay(200);
        }

        private async Task FollowFish()
        {  
            while (await CheckingMiniGameActivity())
            {
                int fishPosition = await _fishTracker.FishTracking();
                _hookController.ControlHook(fishPosition);
            }
            Console.WriteLine("Риба піймана");
        }

        private void WaitForCatchNotification()
        {
            Console.WriteLine("Чекаємо на підтвердження...");
            _messageTracker.TrackMessageCycle(_config.CatchNotificationRegion, _config.CatchNotificationTemplate, _config.CatchNotificationThreshold);
            Console.WriteLine("Повідомлення зенайденно!");
        }
    }
}
