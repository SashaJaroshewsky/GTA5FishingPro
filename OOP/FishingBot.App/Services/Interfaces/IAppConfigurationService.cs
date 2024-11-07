using FishingBot.App.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace FishingBot.App.Services.Interfaces
{
    internal interface IAppConfigurationService
    {
        Region HookSearchRegion { get; }
        Region[] FishSearchRegions { get; }
        Region MiniGameSearchRegion { get; }
        Region CatchNotificationRegion { get; }
        Region FishAlertRegion { get; }
        List<Mat> FishImageTemplate { get; }
        Mat HookImageTemplate { get; }
        Mat MiniGameImageTemplate { get; }
        Mat CatchNotificationTemplate { get; }
        Mat FishAlertTemplate { get; }
        double TemplateMatchThreshold { get; }
        double HookTemplateMatchThreshold { get; }
        double CatchNotificationThreshold { get; }
        double MiniGameNotificationThreshold { get; }
    }
}
