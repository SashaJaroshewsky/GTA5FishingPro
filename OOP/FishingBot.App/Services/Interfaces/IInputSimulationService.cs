using System.Threading.Tasks;
using WindowsInput.Native;

namespace FishingBot.App.Services.Interfaces
{
    internal interface IInputSimulationService
    {
        Task PressKey(VirtualKeyCode keyCode);
        Task ClickLeftMouseButton();
        Task PressRightMouseButton();
        Task ReleaseRightMouseButton();
    }
}
