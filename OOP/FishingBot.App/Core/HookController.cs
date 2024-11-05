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
        public void ControlHook(int fishY)
        {
             _inputSimulation.ClickLeftMouseButton();

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (_imageComparator.CompareImageWithRegion(_config.HookSearchRegion, _config.HookImageTemplate, _config.HookTemplateMatchThreshold, out Point location))
                    {
                        int hookY = location.Y + _config.HookSearchRegion.Y;
                        MoveHookToFish(fishY, hookY);
                    }
                     Task.Delay(1, cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Час на ловлю риби вичерпано. Вихід із методу.");
                return;
            }
        }

        private void MoveHookToFish(int fishY, int hookY)
        {
            if (fishY >= hookY)
            {
                 _inputSimulation.PressRightMouseButton();
            }
            else
            {
                _inputSimulation.ReleaseRightMouseButton();
            }
        }



    }
}
