using FishingBot.App.Models;
using OpenCvSharp;

namespace FishingBot.App.Services.Interfaces
{
    internal interface IScreenCaptureService
    {
        Mat CaptureRegion(Region region);
        Mat CaptureFullScreen();
    }
}
