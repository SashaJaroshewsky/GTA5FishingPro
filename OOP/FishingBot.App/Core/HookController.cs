using FishingBot.App.Configuration;
using FishingBot.App.Services.Interfaces;
using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FishingBot.App.Core
{
    internal class HookController
    {
        private readonly IInputSimulationService _inputSimulation;
        private readonly AppConfiguration _config;
        private readonly IImageComparatorService _imageComparator;

        public HookController(IInputSimulationService inputSimulation, AppConfiguration configuration, IImageComparatorService imageComparator)
        {
            _inputSimulation = inputSimulation;
            _config = configuration;
            _imageComparator = imageComparator;
        }
        public async Task ControlHook(int fishY)
        {
            await _inputSimulation.ClickLeftMouseButton();

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (_imageComparator.CompareImageWithRegion(_config.HookSearchRegion, _config.HookImageTemplate, _config.HookTemplateMatchThreshold, out Point location))
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

        private async Task MoveHookToFish(int fishY, int hookY)
        {
            if (fishY >= hookY)
            {
                await _inputSimulation.PressRightMouseButton();
            }
            else
            {
                await _inputSimulation.ReleaseRightMouseButton();
            }
        }
    }
}
