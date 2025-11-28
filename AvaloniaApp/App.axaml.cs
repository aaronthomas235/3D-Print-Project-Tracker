using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaApp.Services;
using AvaloniaApp.Views;
using Core.Interfaces;
using Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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
                // Create DI container
                var services = new ServiceCollection();

                // Register ViewModels
                services.AddSingleton<MainViewModel>();

                // Create main window
                var mainWindow = new MainWindow();

                // Register Avalonia-specific services
                services.AddSingleton<IFileManagementService, Core.Services.FileManagementService>();
                services.AddSingleton<IFolderSelectionService>(new FolderSelectionService(mainWindow));

                // Build the provider
                serviceProvider = services.BuildServiceProvider();

                desktop.MainWindow = mainWindow;
                // Set window DataContext
                mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}