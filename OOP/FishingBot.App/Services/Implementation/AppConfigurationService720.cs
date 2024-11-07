using FishingBot.App.Models;
using FishingBot.App.Services.Interfaces;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingBot.App.Services.Implementation
{
    internal class AppConfigurationService720: IAppConfigurationService
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

        public AppConfigurationService720()
        {
            HookSearchRegion = new Region(638, 85, 3, 413);
            FishSearchRegions = new[]
            {
                new Region(278, 193, 164, 307),
                new Region(975, 193, 164, 307)
            };
            MiniGameSearchRegion = new Region(573, 31, 134, 21);
            CatchNotificationRegion = new Region(32, 486, 14, 14);
            FishAlertRegion = new Region(1215, 598, 24, 24);

            

            List<string> fishImagePaths = new List<string>
            {
                Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "Fish1.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "Fish2.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "Fish3.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "Fish4.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "Fish5.png"),
                Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "Fish6.png")
            };

            FishImageTemplate = LoadTemplates(fishImagePaths);


            HookImageTemplate = Cv2.ImRead(Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "Hook.png"), ImreadModes.Grayscale);
            MiniGameImageTemplate = Cv2.ImRead(Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "MiniGame.png"), ImreadModes.Grayscale);
            CatchNotificationTemplate = Cv2.ImRead(Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "CatchNotification.png"), ImreadModes.Grayscale);
            FishAlertTemplate = Cv2.ImRead(Path.Combine(AppContext.BaseDirectory, "Resources", "Size720", "FishAlert.png"), ImreadModes.Grayscale);

            TemplateMatchThreshold = 0.8;
            HookTemplateMatchThreshold = 0.7;
            CatchNotificationThreshold = 0.8;
            MiniGameNotificationThreshold = 0.92;
        }


        private List<Mat> LoadTemplates(List<string> paths)
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
