using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingBot.App.Services.Interfaces
{
    internal interface ITemplateMatchingService
    {
        bool CheckTemplateMatch(Mat screen, Mat template, double threshold);
        bool CheckTemplateMatch(Mat screen, Mat template, double threshold, out Point location);
    }
}
