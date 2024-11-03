using FishingBot.App.Configuration;
using FishingBot.App.Services.Interfaces;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace FishingBot.App.Core
{
    internal class FishingBot
    {
        private readonly IScreenCaptureService _screenCapture;
        private readonly ITemplateMatchingService _templateMatching;
        private readonly IInputSimulationService _inputSimulation;
        private readonly AppConfiguration _config;
        private bool _isFishing;

        // Конструктор, який приймає всі необхідні залежності
        public FishingBot(
            IScreenCaptureService screenCapture,
            ITemplateMatchingService templateMatching,
            IInputSimulationService inputSimulation,
            AppConfiguration config)
        {
            _screenCapture = screenCapture;
            _templateMatching = templateMatching;
            _inputSimulation = inputSimulation;
            _config = config;
        }

        // Головний цикл риболовлі
        public async Task StartFishingLoop()
        {
            while (true)
            {
                try
                {
                    _isFishing = true;
                    await StartFishing();
                    await WaitForFish();
                    await StartMiniGame();
                    await Task.Delay(2000);
                    await FollowFish();
                    await WaitForCatchNotification();
                    _isFishing = false;
                    await Task.Delay(400);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка під час риболовлі: {ex.Message}");
                    _isFishing = false;
                    await Task.Delay(1000);
                }
            }
        }

        // Запуск риболовлі (натискання клавіші 1)
        private async Task StartFishing()
        {
            Console.WriteLine("Запуск риболовлі...");
            await _inputSimulation.PressKey(VirtualKeyCode.VK_1);
            await Task.Delay(1000);
        }

        // Очікування появи риби
        private async Task WaitForFish()
        {
            Console.WriteLine("Чекаємо на рибу...");
            while (_isFishing)
            {
                var screen = _screenCapture.CaptureRegion(_config.FishAlertRegion)
                    .CvtColor(ColorConversionCodes.BGR2GRAY);

                if (_templateMatching.CheckTemplateMatch(screen, _config.FishAlertTemplate, _config.TemplateMatchThreshold))
                {
                    Console.WriteLine("Риба з'явилась!");
                    return;
                }
                await Task.Delay(1000);
            }
        }

        // Запуск міні-гри (натискання клавіші E)
        private async Task StartMiniGame()
        {
            Console.WriteLine("Запуск міні-гри...");
            await _inputSimulation.PressKey(VirtualKeyCode.VK_E);
            await Task.Delay(200);
        }

        // Відстеження риби та контроль гачка
        private async Task FollowFish()
        {
            Console.WriteLine("Відстежуємо рибу...");
            while (_isFishing)
            {
                if (await TryFindAndFollowFish())
                {
                    // Перевірка чи міні-гра все ще активна
                    var miniGameScreen = _screenCapture.CaptureRegion(_config.MiniGameSearchRegion)
                        .CvtColor(ColorConversionCodes.BGR2GRAY);

                    if (!_templateMatching.CheckTemplateMatch(
                        miniGameScreen,
                        _config.MiniGameImageTemplate,
                        _config.TemplateMatchThreshold))
                    {
                        Console.WriteLine("Міні гра завершена");
                        return;
                    }
                }
                await Task.Delay(500);
            }
        }

        // Пошук риби та керування гачком
        private async Task<bool> TryFindAndFollowFish()
        {
            foreach (var region in _config.FishSearchRegions)
            {
                var screen = _screenCapture.CaptureRegion(region)
                    .CvtColor(ColorConversionCodes.BGR2GRAY);

                foreach (var template in _config.FishImageTemplate)
                {
                    if (_templateMatching.CheckTemplateMatch(
                        screen,
                        template,
                        _config.TemplateMatchThreshold,
                        out Point location))
                    {
                        await ControlHook(location.Y + region.Y);
                        return true;
                    }
                }
            }
            return false;
        }

        // Керування гачком
        private async Task ControlHook(int fishY)
        {

            await _inputSimulation.ClickLeftMouseButton();
            //await Task.Delay(50);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var screen = _screenCapture.CaptureRegion(_config.HookSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);

                    if (_templateMatching.CheckTemplateMatch(screen, _config.HookImageTemplate, _config.HookTemplateMatchThreshold, out Point location))
                    {
                        int hookY = location.Y + _config.HookSearchRegion.Y;
                        await MoveHookToFish(fishY, hookY);
                    }
                    await Task.Delay(1, cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Час на ловлю риби вичерпано. Вихід із методу.");
                return;
            }



        }

        // Рух гачка відносно риби
        private async Task MoveHookToFish(int fishY, int hookY)
        {
            const int threshold = 0;

            if (fishY >= hookY + threshold)
            {
                Console.WriteLine($"Піднімаємо гачок. Риба: {fishY}, Гачок: {hookY}");
                await _inputSimulation.PressRightMouseButton();
            }
            else
            {
                Console.WriteLine($"Опускаємо гачок. Риба: {fishY}, Гачок: {hookY}");
                await _inputSimulation.ReleaseRightMouseButton();
            }
        }

        // Очікування повідомлення про спійману рибу
        private async Task WaitForCatchNotification()
        {
            Console.WriteLine("Чекаємо на повідомлення про спійману рибу...");
            while (_isFishing)
            {
                var screen = _screenCapture.CaptureRegion(_config.CatchNotificationRegion)
                    .CvtColor(ColorConversionCodes.BGR2GRAY);

                if (_templateMatching.CheckTemplateMatch(
                    screen,
                    _config.CatchNotificationTemplate,
                    _config.TemplateMatchThreshold))
                {
                    Console.WriteLine("Рибу спіймано!");
                    return;
                }
                await Task.Delay(500);
            }
        }

        // Метод для зупинки риболовлі
        public void StopFishing()
        {
            _isFishing = false;
        }
    }
}
