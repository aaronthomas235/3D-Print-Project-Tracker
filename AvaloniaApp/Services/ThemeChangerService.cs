using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using ThreeDPrintProjectTracker.Engine.Interfaces;

namespace ThreeDPrintProjectTracker.Avalonia.Services
{
    public class ThemeChangerService : IThemeChangerService
    {
        public void SetTheme(bool useDarkTheme)
        {
            if (Application.Current is App app)
            {
                App.SetTheme(useDarkTheme);
            }
        }
    }
}
