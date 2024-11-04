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

        public async Task PressKey(VirtualKeyCode keyCode)
        {
            _sim.Keyboard.KeyPress(keyCode);
            await Task.Delay(50);
        }

        public async Task ClickLeftMouseButton()
        {
            lock (_lockObject)
            {
                _sim.Mouse.LeftButtonClick();
            }
            await Task.Delay(50);
        }

        public async Task PressRightMouseButton()
        {
            lock (_lockObject)
            {
                _sim.Mouse.RightButtonDown();
            }
            await Task.Delay(1);
        }

        public async Task ReleaseRightMouseButton()
        {
            lock (_lockObject)
            {
                _sim.Mouse.RightButtonUp();
            }
            await Task.Delay(1);
        }
    }
}
