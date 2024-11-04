using FishingBot.App.Configuration;
using FishingBot.App.Services.Interfaces;
using OpenCvSharp;
using System.Threading.Tasks;

namespace FishingBot.App.Core
{
    internal class MessageTracker
    {

        private readonly IImageComparatorService _imageComparator;

        public MessageTracker(IImageComparatorService imageComparator)
        {
            _imageComparator = imageComparator;
        }

        public async Task<bool> TrackMessage(Models.Region region, Mat target, double threshold)
        {
            while (true)
            {
                if (_imageComparator.CompareImageWithRegion(region, target, threshold))
                    return true;
                await Task.Delay(1000);
            }
        }
    }
}
