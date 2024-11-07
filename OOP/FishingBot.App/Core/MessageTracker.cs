
using FishingBot.App.Services.Interfaces;
using OpenCvSharp;
using System;
using System.Threading;
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

        public async Task TrackMessageCycle(Models.Region region, Mat target, double threshold)
        {
            while (true)
            {
                if (_imageComparator.CompareImageWithRegion(region, target, threshold, out Point l))
                {
                    Console.WriteLine($"location: {l.X} {l.Y}");
                    return;
                }
                await Task.Delay(1000);
            }
        }

        public async Task TrackMessageCycle(Models.Region region, Mat target, double threshold, CancellationToken token)
        {
            while (true)
            {
                if (_imageComparator.CompareImageWithRegion(region, target, threshold, out Point l))
                {
                    Console.WriteLine($"location: {l.X} {l.Y}");
                    return;
                }
                await Task.Delay(1000, token);
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
