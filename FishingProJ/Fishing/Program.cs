using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;
using WindowsInput;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using OpenCvSharp.Extensions;
using System.Collections.Generic;


namespace Fishing
{
    internal class Program
    {
        private static readonly InputSimulator _sim = new InputSimulator();
        private static readonly object _lockObject = new object();
        private static bool _isFishing = false;
        private static int _hookSearchRegionPositionY = 160;// 150;
        private static readonly Rectangle _hookSearchRegion = new Rectangle(958, _hookSearchRegionPositionY, 4, 600);

        private static int _fishSearchRegionPositionY = 274;

        private static readonly Rectangle[] _fishSearchRegion = new Rectangle[]
        {
            new Rectangle(385, _fishSearchRegionPositionY, 246, 460),
            new Rectangle(1420, _fishSearchRegionPositionY, 246, 460)
        };


        private static readonly Rectangle _miniGameSearchRegion = new Rectangle(857, 42, 210, 70);
        private static readonly Rectangle _catchNotificationSearchRegion = new Rectangle(48, 738, 22, 22);
        private static readonly Rectangle _fishAlertSearchRegion = new Rectangle(1831, 893, 6, 28);

        //720p
        //private static int _hookSearchRegionPositionY = 85;
        //private static readonly Rectangle _hookSearchRegion = new Rectangle(638, _hookSearchRegionPositionY, 3, 413);

        //private static int _fishSearchRegionPositionY = 193;
        //private static readonly Rectangle[] _fishSearchRegion = new Rectangle[]
        //{
        //    new Rectangle(278, _fishSearchRegionPositionY, 164, 307),
        //    new Rectangle(975, _fishSearchRegionPositionY, 164, 307)
        //};

        //private static readonly Rectangle _miniGameSearchRegion = new Rectangle(573, 31, 134, 21);
        //private static readonly Rectangle _catchNotificationSearchRegion = new Rectangle(32, 494, 14, 14);
        //private static readonly Rectangle _fishAlertSearchRegion = new Rectangle(1215, 598, 24, 24);



        private static readonly List<string> _fishImagePaths = new List<string>
        {
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish1.png"),
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish2.png"),
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish3.png"),
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish4.png"),
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish5.png"),
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish6.png")
        };



        private static readonly List<Mat> _fishTemplates = LoadTemplates(_fishImagePaths);

        //private static readonly string _hookImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Hook.png");
        //private static readonly string _miniGameImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "MiniGame.png");

        //private static readonly string _catchNotificationPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "CatchNotification.png");

        //private static readonly string _fishAlertPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FishAlert.png");


        private static readonly Mat _hookTemplate = Cv2.ImRead(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Hook.png"), ImreadModes.Grayscale);
        private static readonly Mat _miniGameTemplate = Cv2.ImRead(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "MiniGame.png"), ImreadModes.Grayscale);
        private static readonly Mat _catchNotificationTemplate = Cv2.ImRead(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "CatchNotification.png"), ImreadModes.Grayscale);
        private static readonly Mat _fishAlertTemplate = Cv2.ImRead(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FishAlert.png"), ImreadModes.Grayscale);


        static async Task Main(string[] args)
        {
            Console.WriteLine("Починається процес авто рибо ловлі");
            await Task.Delay(5000);
            // Запускаємо основний цикл риболовлі та моніторинг клавіші виходу
            await Task.WhenAny(
                Task.Run(FishingLoop),
                Task.Run(MonitorExitKey)
            );
        }
        //private static void HotKeyPressed(object sender, KeyPressedEventArgs e)
        //{
        //    // Якщо натиснута гаряча клавіша - виходимо з програми
        //    Console.WriteLine("Гаряча клавіша натиснута. Виходимо з програми...");
        //    Environment.Exit(0);
        //}

        static async Task FishingLoop()
        {
            while (true)
            {
                _isFishing = true;
                await StartFishing();
                await WaitForFish();
                await StartMiniGame();
                await Task.Delay(2000);
                await FollowFish();
                await WaitForCatchNotification();
                _isFishing = false;
                await Task.Delay(400); // Пауза перед початком нового циклу
            }
        }

        static async Task MonitorExitKey()
        {
            Console.WriteLine("Натисніть 'Q' для виходу.");

            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }

                await Task.Delay(200); // Асинхронна затримка замість Thread.Sleep
            }
        }


        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        //private const int KEYEVENTF_KEYDOWN = 0x0000; // Натискання клавіші
        //private const int KEYEVENTF_KEYUP = 0x0002;   // Відпускання клавіші
        //private const byte VK_1 = 0x31;               // Код клавіші 1
        //private const byte VK_E = 0x45;

        //static async void PressKey(byte keyCode)
        //{
        //    keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, 0); // Натискання клавіші
        //    await Task.Delay(50); // Невелика затримка між натисканням і відпусканням
        //    keybd_event(keyCode, 0, KEYEVENTF_KEYUP, 0);   // Відпускання клавіші
        //}

        static async Task StartFishing()
        {
            Console.WriteLine("Запуск риболовлі...");

            _sim.Keyboard.KeyPress(VirtualKeyCode.VK_1);
            //PressKey(VK_1);

            await Task.Delay(1000);
        }

        static async Task WaitForFish()
        {
            Console.WriteLine("Чекаємо на рибу...");
            //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FishAlert.png");


            while (_isFishing)
            {
                //var screen = CaptureBottomRightCorner();
                var screen = CaptureRegion(_fishAlertSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);
                //var screen = CaptureFullScreen(/*_fishAlertSearchRegion*/).CvtColor(ColorConversionCodes.BGR2GRAY);
                if (CheckTemplateMatch(screen, _fishAlertTemplate, 0.8, out OpenCvSharp.Point location))
                {
                    Console.WriteLine($"Alert = {location.X} {location.Y}");
                    return;
                }

                //using (var result = new Mat())
                //{
                //    Cv2.MatchTemplate(screen, _fishAlertTemplate, result, TemplateMatchModes.CCoeffNormed);
                //    Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point loc);

                //    if (maxVal >= 0.9)
                //    {
                //        Console.WriteLine($"Риба знайдена! x={loc.X}, y={loc.Y}");
                //        return;
                //    }
                //}
                await Task.Delay(1000);
            }

        }

        static async Task StartMiniGame()
        {
            Console.WriteLine("Запуск міні-гри...");
            _sim.Keyboard.KeyPress(VirtualKeyCode.VK_E);

            await Task.Delay(200);
        }

        static async Task FollowFish()
        {
            Console.WriteLine("Відстежуємо рибу...");

            while (_isFishing)
            {
                //var screen = CaptureFullScreen().CvtColor(ColorConversionCodes.BGR2GRAY);

                //Mat[] screens = new Mat[] { CaptureRegion(_fishSearchRegion[0]).CvtColor(ColorConversionCodes.BGR2GRAY), CaptureRegion(_fishSearchRegion[1]).CvtColor(ColorConversionCodes.BGR2GRAY) };
                ////var screen = CaptureRegion(_fishSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);

                ////var screen = CaptureFullScreen().CvtColor(ColorConversionCodes.BGR2GRAY);
                ////var screenMiniGame = CaptureFullScreen().CvtColor(ColorConversionCodes.BGR2GRAY);
                //foreach (var screen in screens)
                //{
                //    foreach (var fishTemplate in _fishTemplates)
                //    {
                //        if (CheckTemplateMatch(screen, fishTemplate, 0.7, out OpenCvSharp.Point location))
                //        {
                //            Console.WriteLine("Риба пливе");
                //            await HookControlLoop(location.Y + _fishSearchRegionPositionY + 10);
                //            break;
                //        }
                //    }

                //}
                if (SearchingFish(out OpenCvSharp.Point location))
                {
                    Console.WriteLine("Риба знайдена!");
                    await HookControlLoop(location.Y + _fishSearchRegionPositionY + 10);
                }


                var screenMiniGame = CaptureRegion(_miniGameSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);
                //var screenMiniGame = CaptureFullScreen().CvtColor(ColorConversionCodes.BGR2GRAY);
                if (!CheckTemplateMatch(screenMiniGame, _miniGameTemplate, 0.8))
                {
                    Console.WriteLine($"Міні гра завершена. Починаю рибалити знову");
                    return;
                }
                await Task.Delay(500);

            }
        }

        private static bool SearchingFish(out OpenCvSharp.Point loc)
        {
            Mat[] screens = new Mat[]
            { CaptureRegion(_fishSearchRegion[0]).CvtColor(ColorConversionCodes.BGR2GRAY),
              CaptureRegion(_fishSearchRegion[1]).CvtColor(ColorConversionCodes.BGR2GRAY)
            };

            for (int i = 0; i <=2; i++)
            {
                if(CheckTemplateMatch(screens[0], _fishTemplates[i], 0.8, out OpenCvSharp.Point location))
                {
                    loc = location;
                    return true;
                }
            }

            for (int i = 3; i <= 5; i++)
            {
                if (CheckTemplateMatch(screens[1], _fishTemplates[i], 0.8, out OpenCvSharp.Point location))
                {
                    loc = location;
                    return true;
                }
                   
            }

            loc = new OpenCvSharp.Point();
            return false;
        }

        static async Task WaitForCatchNotification()
        {
            Console.WriteLine("Чекаємо на повідомлення про спійману рибу...");


            while (_isFishing)
            {
                var screen = CaptureRegion(_catchNotificationSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);
                //var screen = CaptureFullScreen().CvtColor(ColorConversionCodes.BGR2GRAY);
                if (CheckTemplateMatch(screen, _catchNotificationTemplate, 0.8/*, out OpenCvSharp.Point location*/))
                {
                    Console.WriteLine($"Повідомлення про спійману рибу знайдено!");
                    //Console.WriteLine($"Повідомлення про спійману рибу знайдено! {location.X} {location.Y}");
                    return;
                }
                //using (var result = new Mat())
                //{
                //    Cv2.MatchTemplate(screen, _catchNotificationTemplate, result, TemplateMatchModes.CCoeffNormed);
                //    Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point matchLoc);

                //    if (maxVal >= 0.8)
                //    {
                //        Console.WriteLine($"Повідомлення про спійману рибу знайдено! x={matchLoc.X}, y={matchLoc.Y}");
                //        await Task.Delay(1000);
                //        return;
                //    }
                //}
                await Task.Delay(500);

            }
        }

        static async Task HookControlLoop(int fishYPosition)
        {
            bool isFishDetected = true;
            _sim.Mouse.LeftButtonClick();
            await Task.Delay(50);

            using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
            {
                try
                {
                    while (isFishDetected && _isFishing && !cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var screen = CaptureRegion(_hookSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);

                        //using (var hookTemplate = Cv2.ImRead(_hookImagePath, ImreadModes.Grayscale))
                        //{

                        if (CheckTemplateMatch(screen, _hookTemplate, 0.7, out OpenCvSharp.Point location))
                        {
                            Console.WriteLine($"гачок знайдено на позиції {location.X}");
                            await MoveHookToFish(fishYPosition, location.Y + _hookSearchRegionPositionY);
                        }
                        else {
                            Console.WriteLine($"гачок не знайдено на позиції");
                            //return;
                        }

                        
                        //int hookY = GetHookPositionY(screen, _hookTemplate);
                        //if (hookY != -1)
                        //{
                        //    Console.WriteLine($"Рухаємо гачок. Риба на позиції: Y = {fishYPosition}, Гачок на позиції: Y = {hookY}");
                        //    await MoveHookToFish(fishYPosition, hookY);
                        //}
                        //else
                        //{
                        //    return; // Вихід, якщо гачок не знайдено
                        //}
                        //}
                        await Task.Delay(1, cancellationTokenSource.Token); // Невелика затримка для контролю частоти оновлення
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Час на ловлю риби вичерпано. Вихід із методу.");
                    return;
                }
            }
        }

        static async Task MoveHookToFish(int fishY, int hookY)
        {
            //if (hookY == -1) return;

            const int threshold = 0; // Зменшено поріг для більш точного керування

            if (fishY >= hookY + threshold)
            {
                Console.WriteLine($"Піднімаємо гачок. Риба: {fishY}, Гачок: {hookY}");
                await PressRightMouseButton();
            }
            else
            {
                Console.WriteLine($"Опускаємо гачок. Риба: {fishY}, Гачок: {hookY}");
                await ReleaseRightMouseButton();
            }
        }

        static async Task PressRightMouseButton()
        {
            lock (_lockObject)
            {
                _sim.Mouse.RightButtonDown();
            }
            await Task.Delay(1); // Мінімальна затримка для стабільності
        }

        static async Task ReleaseRightMouseButton()
        {
            lock (_lockObject)
            {
                _sim.Mouse.RightButtonUp();
            }
            await Task.Delay(1); // Мінімальна затримка для стабільності
        }

        static Mat CaptureRegion(Rectangle region)
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

        static Mat CaptureFullScreen()
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

        static bool CheckTemplateMatch(Mat screen, Mat template, double threshold)
        {
            using (var result = new Mat())
            {
                Cv2.MatchTemplate(screen, template, result, TemplateMatchModes.CCoeffNormed /*TemplateMatchModes.SqDiff*/);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out _);
                return maxVal >= threshold;
            }
        }

        //static bool CheckTemplateMatch(Mat screen, Mat template, double thresholdPercentage)
        //{
        //    using (var result = new Mat())
        //    {
        //        Cv2.MatchTemplate(screen, template, result, TemplateMatchModes.SqDiff);
        //        Cv2.MinMaxLoc(result, out double minVal, out _, out _, out _);

        //        // Обчислюємо максимальне можливе значення для шаблону
        //        double maxPossibleValue = Math.Pow(255.0, 2) * (template.Rows * template.Cols);

        //        // Обчислюємо відсоток відповідності
        //        double similarity = 1 - (minVal / maxPossibleValue);

        //        // Переводимо відсоток у значення від 0 до 100
        //        double similarityPercentage = similarity;

        //        Console.WriteLine($"Відсоток відповідності: {similarityPercentage}%");

        //        return similarityPercentage >= 0;
        //    }
        //}

        static bool CheckTemplateMatch(Mat screen, Mat template, double threshold, out OpenCvSharp.Point Location)
        {
            using (var result = new Mat())
            {
                Cv2.MatchTemplate(screen, template, result, TemplateMatchModes.CCoeffNormed /*TemplateMatchModes.SqDiff*/);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point Loc);
                Location = Loc;

                Console.WriteLine($"Співпадіння = {maxVal}");
                return maxVal >= threshold;
            }
        }


        static bool CheckMiniGame(Mat screen, Mat miniGameTemplate)
        {
            using (var result = new Mat())
            {
                Cv2.MatchTemplate(screen, miniGameTemplate, result, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point hookLoc);

                if (maxVal >= 0.8)
                {
                    Console.WriteLine($"Міні гра запущена. Точність = {maxVal}. x={hookLoc.X}, y={hookLoc.Y}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Міні гра закрита. Точність = {maxVal}");
                    return false;
                }
            }
        }

        static int GetHookPositionY(Mat screen, Mat hookTemplate)
        {
            using (var result = new Mat())
            {

                Cv2.MatchTemplate(screen, hookTemplate, result, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point hookLoc);

                if (maxVal >= 0.9)
                {
                    Console.WriteLine($"Гачок знайдено на позиції: Y = {hookLoc.Y}, Впевненість: {maxVal:F2}");
                    return hookLoc.Y + _hookSearchRegionPositionY;
                }
                else
                {
                    Console.WriteLine($"Гачок не знайдено. Найкраща відповідність: {maxVal:F2}");
                    return -1;
                }
            }
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