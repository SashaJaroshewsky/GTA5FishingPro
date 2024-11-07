using FishingBot.App.Services.Interfaces;
using OpenCvSharp;
using System;

namespace FishingBot.App.Services.Implementation
{
    internal class ImageComparatorService : IImageComparatorService
    {
        private readonly IScreenCaptureService _screenCapture;
        private readonly ITemplateMatchingService _templateMatching;

        public ImageComparatorService(IScreenCaptureService screenCaptureService, ITemplateMatchingService templateMatchingService)
        {
            _screenCapture = screenCaptureService;
            _templateMatching = templateMatchingService;
        }

        public bool CompareImageWithRegion(Models.Region region, Mat target, double threshold)
        {
            using (var screen = _screenCapture.CaptureRegion(region).CvtColor(ColorConversionCodes.BGR2GRAY))
            {
                if (_templateMatching.CheckTemplateMatch(screen, target, threshold))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CompareImageWithRegion(Models.Region region, Mat target, double threshold, out Point location)
        {
            using (var screen = _screenCapture.CaptureRegion(region).CvtColor(ColorConversionCodes.BGR2GRAY))
            {
                if (_templateMatching.CheckTemplateMatch(screen, target, threshold, out Point loc))
                {
                    location = loc;
                    return true;
                }
                location = loc;
                return false;
            }
        }
    }
}
