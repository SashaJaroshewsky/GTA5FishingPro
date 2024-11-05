using System.Threading.Tasks;
using WindowsInput.Native;
using WindowsInput;
using FishingBot.App.Services.Interfaces;

namespace FishingBot.App.Services.Implementation
{
    internal class InputSimulationService: IInputSimulationService
    {
        private readonly InputSimulator _sim;
        private readonly object _lockObject = new object();

        public InputSimulationService()
        {
            _sim = new InputSimulator();
        }

        public void PressKey(VirtualKeyCode keyCode)
        {
            _sim.Keyboard.KeyPress(keyCode);
            Task.Delay(50);
        }

        public void ClickLeftMouseButton()
        {
            lock (_lockObject)
            {
                _sim.Mouse.LeftButtonClick();
            }
            Task.Delay(50);
        }

        public void PressRightMouseButton()
        {
            lock (_lockObject)
            {
                _sim.Mouse.RightButtonDown();
            }
            Task.Delay(1);
        }

        public void ReleaseRightMouseButton()
        {
            lock (_lockObject)
            {
                _sim.Mouse.RightButtonUp();
            }
            Task.Delay(1);
        }
    }
}
