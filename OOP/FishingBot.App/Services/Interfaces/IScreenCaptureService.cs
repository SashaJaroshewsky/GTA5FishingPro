using FishingBot.App.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingBot.App.Services.Interfaces
{
    internal interface IScreenCaptureService
    {
        Mat CaptureRegion(Region region);
        Mat CaptureFullScreen();
    }
}
