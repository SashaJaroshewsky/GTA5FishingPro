using FishingBot.App.Services.Interfaces;
using OpenCvSharp;

namespace FishingBot.App.Services.Implementation
{
    internal class TemplateMatchingService: ITemplateMatchingService
    {
        public bool CheckTemplateMatch(Mat screen, Mat template, double threshold)
        {
            using (var result = new Mat())
            {
                Cv2.MatchTemplate(screen, template, result, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out _);
                return maxVal >= threshold;
            }
        }

        public bool CheckTemplateMatch(Mat screen, Mat template, double threshold, out Point location)
        {
            using (var result = new Mat())
            {
                Cv2.MatchTemplate(screen, template, result, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out Point loc);
                location = loc;
                return maxVal >= threshold;
            }
        }
    }
}
