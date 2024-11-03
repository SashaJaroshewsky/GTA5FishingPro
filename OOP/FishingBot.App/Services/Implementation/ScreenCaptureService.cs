using OpenCvSharp.Extensions;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FishingBot.App.Services.Interfaces;

namespace FishingBot.App.Services.Implementation
{
    internal class ScreenCaptureService: IScreenCaptureService
    {
        public Mat CaptureRegion(Models.Region region)
        {
            using (var bmp = new Bitmap(region.Width, region.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(region.X, region.Y, 0, 0, bmp.Size);
                }
                return BitmapConverter.ToMat(bmp);
            }
        }

        public Mat CaptureFullScreen()
        {
            using (var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                }
                return BitmapConverter.ToMat(bmp);
            }
        }
    }
}
