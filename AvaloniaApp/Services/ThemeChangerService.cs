using System;
using Avalonia;
using Avalonia.Styling;
using ThreeDPrintProjectTracker.Engine.Interfaces;

namespace ThreeDPrintProjectTracker.Avalonia.Services
{
    public class ThemeChangerService : IThemeChangerService
    {
        public void SetTheme(bool useDarkTheme)
        {
            if (Application.Current is App app)
            {
                app.RequestedThemeVariant = useDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
                //App.SetTheme(useDarkTheme);
            }
        }
    }
}
