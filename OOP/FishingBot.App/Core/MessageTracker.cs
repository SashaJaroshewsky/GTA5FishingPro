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

        public bool TrackMessageCycle(Models.Region region, Mat target, double threshold)
        {
            while (true)
            {
                if (_imageComparator.CompareImageWithRegion(region, target, threshold))
                    return true;
                Task.Delay(1000);
            }
        }

        public bool TrackMessage(Models.Region region, Mat target, double threshold)
        {
            //if (_imageComparator.CompareImageWithRegion(region, target, threshold))
            //    return true;
            //return false;
            return _imageComparator.CompareImageWithRegion(region, target, threshold);
        }
    }
}
