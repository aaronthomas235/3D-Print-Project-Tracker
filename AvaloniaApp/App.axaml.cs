using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using AvaloniaApp.Services;
using AvaloniaApp.Views;
using Core.Interfaces;
using Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace AvaloniaApp
{
    public partial class App : Application
    {
        public static ServiceProvider? serviceProvider {  get; private set; }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {

                var services = new ServiceCollection();

                services.AddSingleton<MainViewModel>();

                var mainWindow = new MainWindow();

                services.AddSingleton<IFileManagementService, Core.Services.FileManagementService>();
                services.AddSingleton<IThemeChangerService, ThemeChangerService>();
                services.AddSingleton<IFolderSelectionService>(new FolderSelectionService(mainWindow));

                serviceProvider = services.BuildServiceProvider();

                desktop.MainWindow = mainWindow;

                mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }

        public void SetTheme(bool useDark)
        {
            var app = Application.Current;
            if (app == null) return;

            var asm = Assembly.GetEntryAssembly()?.GetName().Name ?? "AvaloniaApp";
            var themeUri = new Uri($"avares://{asm}/Themes/Colours{(useDark ? "Dark" : "Light")}.axaml");

            app.Resources.MergedDictionaries.Clear();

            var dict = (ResourceDictionary)AvaloniaXamlLoader.Load(themeUri);
            app.Resources.MergedDictionaries.Add(dict);
        }


    }
}