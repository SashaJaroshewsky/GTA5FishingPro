using System.Threading.Tasks;
using WindowsInput.Native;

namespace FishingBot.App.Services.Interfaces
{
    internal interface IInputSimulationService
    {
        void PressKey(VirtualKeyCode keyCode);
        void ClickLeftMouseButton();
        void PressRightMouseButton();
        void ReleaseRightMouseButton();
    }
}
