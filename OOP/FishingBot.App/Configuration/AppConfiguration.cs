using FishingBot.App.Models;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace FishingBot.App.Configuration
{
    internal class AppConfiguration
    {
        public Region HookSearchRegion { get; }
        public Region[] FishSearchRegions { get; }
        public Region MiniGameSearchRegion { get; }
        public Region CatchNotificationRegion { get; }
        public Region FishAlertRegion { get; }
        public List<Mat> FishImageTemplate { get; }
        public Mat HookImageTemplate { get; }
        public Mat MiniGameImageTemplate { get; }
        public Mat CatchNotificationTemplate { get; }
        public Mat FishAlertTemplate { get; }
        public double TemplateMatchThreshold { get; }

        public double HookTemplateMatchThreshold { get; }
        public double CatchNotificationThreshold { get; }
        public double MiniGameNotificationThreshold { get; }

        public AppConfiguration()
        {
            HookSearchRegion = new Region(958, 160, 4, 600);
            FishSearchRegions = new[]
            {
                new Region(385, 274, 246, 460),
                new Region(1420, 274, 246, 460)
            };
            MiniGameSearchRegion = new Region(857, 42, 210, 70);
            CatchNotificationRegion = new Region(48, 738, 22, 22);
            FishAlertRegion = new Region(1832, 893, 6, 28);

            List<string> fishImagePaths = new List<string>
            {
                Path.Combine(AppContext.BaseDirectory, "Resources", "Fish1.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Fish2.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Fish3.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Fish4.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Fish5.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Fish6.png")
            };

            FishImageTemplate = LoadTemplates(fishImagePaths);
            HookImageTemplate = Cv2.ImRead(Path.Combine(AppContext.BaseDirectory, "Resources", "Hook.png"), ImreadModes.Grayscale);
            MiniGameImageTemplate = Cv2.ImRead(Path.Combine(AppContext.BaseDirectory, "Resources", "MiniGame.png"), ImreadModes.Grayscale);
            CatchNotificationTemplate = Cv2.ImRead(Path.Combine(AppContext.BaseDirectory, "Resources", "CatchNotification.png"), ImreadModes.Grayscale);
            FishAlertTemplate = Cv2.ImRead(Path.Combine(AppContext.BaseDirectory, "Resources", "FishAlert.png"), ImreadModes.Grayscale);

            TemplateMatchThreshold = 0.8;
            HookTemplateMatchThreshold = 0.7;
            CatchNotificationThreshold = 0.9;
            MiniGameNotificationThreshold = 0.92;
        }


        static List<Mat> LoadTemplates(List<string> paths)
        {
            var templates = new List<Mat>();
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    templates.Add(Cv2.ImRead(path, ImreadModes.Grayscale));
                }
                else
                {
                    Console.WriteLine($"Файл {path} не знайдено.");
                }
            }
            return templates;
        }
    }
}
