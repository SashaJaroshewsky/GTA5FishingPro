using OpenCvSharp;

namespace FishingBot.App.Services.Interfaces
{
    internal interface IImageComparatorService
    {
        bool CompareImageWithRegion(Models.Region region, Mat target, double threshold);
        bool CompareImageWithRegion(Models.Region region, Mat target, double threshold, out Point location);
    }
}
