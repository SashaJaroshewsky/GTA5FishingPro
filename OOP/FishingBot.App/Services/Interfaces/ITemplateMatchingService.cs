using OpenCvSharp;

namespace FishingBot.App.Services.Interfaces
{
    internal interface ITemplateMatchingService
    {
        bool CheckTemplateMatch(Mat screen, Mat template, double threshold);
        bool CheckTemplateMatch(Mat screen, Mat template, double threshold, out Point location);
    }
}
