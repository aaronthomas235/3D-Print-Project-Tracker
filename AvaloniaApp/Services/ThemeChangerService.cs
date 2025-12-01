using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Core.Interfaces;

namespace AvaloniaApp.Services
{
    public class ThemeChangerService : IThemeChangerService
    {
        public void SetTheme(bool useDarkTheme)
        {
            if (Application.Current is App app)
            {
                app.SetTheme(useDarkTheme);
            }
        }
    }
}
