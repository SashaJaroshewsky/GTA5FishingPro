using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
