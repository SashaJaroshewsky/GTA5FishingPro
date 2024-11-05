using FishingBot.App.Configuration;
using FishingBot.App.Services.Interfaces;
using OpenCvSharp;
using System.Threading.Tasks;

namespace FishingBot.App.Core
{
    internal class FishTracker
    {
        private readonly IScreenCaptureService _screenCapture;
        private readonly AppConfiguration _config;
        private readonly ITemplateMatchingService _templateMatching;
        private readonly IInputSimulationService _inputSimulationService;

        public FishTracker(IScreenCaptureService screenCapture,AppConfiguration config,ITemplateMatchingService templateMatching, IInputSimulationService inputSimulationService)
        {
            _screenCapture = screenCapture;
            _config = config;
            _templateMatching = templateMatching;
            _inputSimulationService = inputSimulationService;
        }

        public async Task<int> FishTracking()
        {
             //   _inputSimulationService.PressRightMouseButton();
            while (true) 
            {
                var screen1 = _screenCapture.CaptureRegion(_config.FishSearchRegions[0]).CvtColor(ColorConversionCodes.BGR2GRAY);
                for (int i = 0; i <= 2; i++)
                {
                    if (_templateMatching.CheckTemplateMatch(screen1, _config.FishImageTemplate[i], _config.TemplateMatchThreshold, out Point location))
                    {
                       // _inputSimulationService.ReleaseRightMouseButton();
                        return location.Y + _config.FishSearchRegions[0].Y;
                    }
                }
                var screen2 = _screenCapture.CaptureRegion(_config.FishSearchRegions[1]).CvtColor(ColorConversionCodes.BGR2GRAY);
                for (int i = 3; i <= 5; i++)
                {
                    if (_templateMatching.CheckTemplateMatch(screen2, _config.FishImageTemplate[i], _config.TemplateMatchThreshold, out Point location))
                    {
                       // _inputSimulationService.ReleaseRightMouseButton();
                        return location.Y + _config.FishSearchRegions[1].Y;
                    }
                }
                await Task.Delay(500);
            }
        }

    }
}
