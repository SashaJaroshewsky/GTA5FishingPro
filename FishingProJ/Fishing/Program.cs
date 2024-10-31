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
        private static int _fishSearchRegionPositionY = 253;
        private static int _hookSearchRegionPositionY = 150;
        private static readonly Rectangle _hookSearchRegion = new Rectangle(939, _hookSearchRegionPositionY, 42, 620);
        //private static readonly Rectangle _fishSearchRegion = new Rectangle(338, _fishSearchRegionPositionY, 1400, 500);

        private static readonly Rectangle _fishSearchRegion = new Rectangle(375, _fishSearchRegionPositionY, 1353, 500);

        private static readonly Rectangle _miniGameSearchRegion = new Rectangle(857, 42, 210, 70);
        private static readonly Rectangle _catchNotificationSearchRegion = new Rectangle(678, 957, 562, 64);
        private static readonly Rectangle _fishAlertSearchRegion = new Rectangle(1861, 937, 36, 36);// Задайте координати та розміри
        


        private static readonly List<string> _fishImagePaths = new List<string>
            {
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish1.png"),
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish2.png"),
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish3.png"),
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish4.png"),
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish5.png"),
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Fish6.png")
            };

        private static readonly string _hookImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Hook.png");
        private static readonly string _miniGameImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "MiniGame.png");

        private static readonly string _catchNotificationPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "CatchNotification.png");

        private static readonly string _fishAlertPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FishAlert.png");

        //private static HotKeyManager _hotKeyManager;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Починається автоматизація риболовлі...");
            //_hotKeyManager = new HotKeyManager();
            //_hotKeyManager.Register(Key.Q, ModifierKeys.Control | ModifierKeys.Shift);
            //_hotKeyManager.KeyPressed += HotKeyPressed;
            await Task.Delay(5000);
            // Запускаємо основний цикл риболовлі та моніторинг клавіші виходу
            await Task.WhenAny(
                Task.Run(FishingLoop)
                //Task.Run(MonitorExitKey)
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
                await FollowFish();
                await WaitForCatchNotification();
                _isFishing = false;
                await Task.Delay(400); // Пауза перед початком нового циклу
            }
        }

        //static async Task MonitorExitKey()
        //{
        //    Console.WriteLine("Натисніть 'Q' для виходу.");

        //    while (true)
        //    {
        //        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
        //        {
        //            Environment.Exit(0);
        //        }

        //        await Task.Delay(200); // Асинхронна затримка замість Thread.Sleep
        //    }
        //}

        static async Task StartFishing()
        {
            Console.WriteLine("Запуск риболовлі...");
           
            _sim.Keyboard.KeyPress(VirtualKeyCode.VK_1);
            await Task.Delay(1000);
        }

        static async Task WaitForFish()
        {
            Console.WriteLine("Чекаємо на рибу...");
            //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FishAlert.png");

            if (!File.Exists(_fishAlertPath))
            {
                throw new FileNotFoundException($"Файл {_fishAlertPath} не знайдено.");
            }

            using (var fishTemplate = Cv2.ImRead(_fishAlertPath, ImreadModes.Grayscale))
            {
                while (_isFishing)
                {
                    //var screen = CaptureBottomRightCorner();
                    var screen = CaptureRegion(_fishAlertSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);
                    //var screen = CaptureFullScreen(/*_fishAlertSearchRegion*/).CvtColor(ColorConversionCodes.BGR2GRAY);


                    using (var result = new Mat())
                    {
                        Cv2.MatchTemplate(screen, fishTemplate, result, TemplateMatchModes.CCoeffNormed);
                        Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point loc);

                        if (maxVal >= 0.9)
                        {
                            Console.WriteLine($"Риба знайдена! x={loc.X}, y={loc.Y}");
                            return;
                        }
                    }
                    await Task.Delay(1000);
                }
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
            

            var fishTemplates = LoadTemplates(_fishImagePaths);
            var miniGameTemplates = Cv2.ImRead(_miniGameImagePath, ImreadModes.Grayscale);
            if (fishTemplates.Count == 0 || !File.Exists(_hookImagePath))
            {
                throw new FileNotFoundException("Не знайдено шаблони рибок або гачка.");
            }

            using (var hookTemplate = Cv2.ImRead(_hookImagePath, ImreadModes.Grayscale))
            {
               
                while (_isFishing)
                {
                    //var screen = CaptureFullScreen().CvtColor(ColorConversionCodes.BGR2GRAY);
                    var screen = CaptureRegion(_fishSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);
                    var screenMiniGame = CaptureRegion(_miniGameSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY); ;
                    //var screenMiniGame = CaptureFullScreen().CvtColor(ColorConversionCodes.BGR2GRAY);
                    foreach (var fishTemplate in fishTemplates)
                    {
                        using (var result = new Mat())
                        {
                            Cv2.MatchTemplate(screen, fishTemplate, result, TemplateMatchModes.CCoeffNormed);
                            Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point fishLoc);

                            if (maxVal >= 0.8)
                            {
                                
                                await HookControlLoop(fishLoc.Y + _fishSearchRegionPositionY+10);
                                break;
                            }
                        }
                    }

                    if (!CheckMiniGame(screenMiniGame, miniGameTemplates))
                    {
                        Console.WriteLine("Міні гра завершена. Починаю рибалити знову");
                        return;
                    }
                    await Task.Delay(100);
                }
            }
        }

        static async Task WaitForCatchNotification()
        {
            Console.WriteLine("Чекаємо на повідомлення про спійману рибу...");
            

            if (!File.Exists(_catchNotificationPath))
            {
                throw new FileNotFoundException($"Файл {_catchNotificationPath} не знайдено.");
            }

            using (var catchTemplate = Cv2.ImRead(_catchNotificationPath, ImreadModes.Grayscale))
            {
                while (_isFishing)
                {
                    //var screen = CaptureRegion(_catchNotificationSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);
                    var screen = CaptureFullScreen().CvtColor(ColorConversionCodes.BGR2GRAY);
                    using (var result = new Mat())
                    {
                        Cv2.MatchTemplate(screen, catchTemplate, result, TemplateMatchModes.CCoeffNormed);
                        Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point matchLoc);

                        if (maxVal >= 0.8)
                        {
                            Console.WriteLine($"Повідомлення про спійману рибу знайдено! x={matchLoc.X}, y={matchLoc.Y}");
                            await Task.Delay(1000);
                            return;
                        }
                    }
                    await Task.Delay(500);
                }
            }
        }

        static async Task HookControlLoop(int fishYPosition)
        {
            bool isFishDetected = true;
            _sim.Mouse.RightButtonClick();

            using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
            {
                try
                {
                    while (isFishDetected && _isFishing && !cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var screen = CaptureRegion(_hookSearchRegion).CvtColor(ColorConversionCodes.BGR2GRAY);
                        using (var hookTemplate = Cv2.ImRead(_hookImagePath, ImreadModes.Grayscale))
                        {
                            int hookY = GetHookPositionY(screen, hookTemplate);
                            if (hookY != -1)
                            {
                                Console.WriteLine($"Рухаємо гачок. Риба на позиції: Y = {fishYPosition}, Гачок на позиції: Y = {hookY}");
                                await MoveHookToFish(fishYPosition, hookY);
                            }
                            else
                            {
                                return; // Вихід, якщо гачок не знайдено
                            }
                        }
                        await Task.Delay(10, cancellationTokenSource.Token); // Невелика затримка для контролю частоти оновлення
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Час на ловлю риби вичерпано. Вихід із методу.");
                }
            }
        }

        static async Task MoveHookToFish(int fishY, int hookY)
        {
            if (hookY == -1) return;
            
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

       

        static bool CheckMiniGame(Mat screen, Mat miniGameTemplate)
        {
            using(var result = new Mat())
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