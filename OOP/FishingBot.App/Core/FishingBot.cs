
using FishingBot.App.Services.Implementation;
using FishingBot.App.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace FishingBot.App.Core
{
    internal class FishingBot
    {
        private readonly IInputSimulationService _inputSimulation;
        private readonly IAppConfigurationService _config;
        private readonly FishTracker _fishTracker;
        private readonly HookController _hookController;
        private readonly MessageTracker _messageTracker;

        

        public FishingBot(
            IInputSimulationService inputSimulation,
            IAppConfigurationService config,
            MessageTracker messageTracker,
            FishTracker fishTracker,
            HookController hookController
            )
        {
            _inputSimulation = inputSimulation;
            _config = config;
            _messageTracker = messageTracker;
            _fishTracker = fishTracker;
            _hookController = hookController;
           ;
        }

        public async Task StartFishingLoop(CancellationToken token)
        {
            Console.WriteLine("Починається процес автоматичної риболовлі");


            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(10);
                    await CastFishingRod();
                    await Task.Delay(10);
                    await WaitForFish();
                    await StartMiniGame();
                    await Task.Delay(1000);
                    await FollowFish();
                    await WaitForCatchNotification(token);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Риболовлю зупинено вручну");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка під час риболовлі: {ex.Message}");
                }
            }
        }

        private async Task CastFishingRod()
        {
            Console.WriteLine("Закидаю вудку...");
            _inputSimulation.PressKey(VirtualKeyCode.VK_1);
            await Task.Delay(1000);
        }




        private async Task WaitForFish()
        {
            Console.WriteLine("Чекаємо на рибу...");

            
            await _messageTracker.TrackMessageCycle( _config.FishAlertRegion, _config.FishAlertTemplate, _config.TemplateMatchThreshold);
        }

        private async Task<bool> CheckingMiniGameActivity()
        {
            await Task.Delay(500);
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

        private async Task WaitForCatchNotification(CancellationToken token)
        {
           
            Console.WriteLine("Чекаємо на підтвердження...");
            await _messageTracker.TrackMessageCycle(_config.CatchNotificationRegion, _config.CatchNotificationTemplate, _config.CatchNotificationThreshold, token);
        }
    }
}
