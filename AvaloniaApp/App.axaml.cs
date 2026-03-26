using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using ThreeDPrintProjectTracker.Avalonia.Services;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;
using ThreeDPrintProjectTracker.Avalonia.Views;
using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace ThreeDPrintProjectTracker.Avalonia
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

                services.AddSingleton(desktop);

                ConfigureServices(services);
                ConfigureTransients(services);

                serviceProvider = services.BuildServiceProvider();

                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

                desktop.MainWindow = mainWindow;

                mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<IWindowCreationService, WindowCreationService>();
            services.AddSingleton<IFileLauncherService, FileLauncherService>();
            services.AddSingleton<IThemeChangerService, ThemeChangerService>();

            services.AddSingleton<IFolderSelectionService>(sp => new FolderSelectionService(sp.GetRequiredService<MainWindow>()));

            services.AddSingleton<IProjectTreeCoordinationService, ThreeDPrintProjectTracker.Engine.Services.ProjectTreeCoordinationService>();
            services.AddSingleton<IProjectTreeBuilderService, ThreeDPrintProjectTracker.Engine.Services.ProjectTreeBuilderService>();
            services.AddSingleton<IProjectTreeItemViewModelFactory, ThreeDPrintProjectTracker.Engine.Factories.ProjectTreeItemViewModelFactory>();
            services.AddSingleton<IFileManagementService, ThreeDPrintProjectTracker.Engine.Services.FileManagementService>();
            services.AddSingleton<ISupportedFileFormatsService, ThreeDPrintProjectTracker.Engine.Services.SupportedFileFormatsService>();
            services.AddSingleton<IPrinterProfileService, ThreeDPrintProjectTracker.Engine.Services.PrinterProfileService>();
            services.AddSingleton<IPrintTimeEstimationService, ThreeDPrintProjectTracker.Engine.Services.PrintTimeEstimationService>();
            services.AddSingleton<IMeshAnalyserService, ThreeDPrintProjectTracker.Engine.Services.MeshAnalyserService>();
            services.AddSingleton<IMaterialUsageEstimationService, ThreeDPrintProjectTracker.Engine.Services.MaterialUsageEstimationService>();
            services.AddSingleton<IPrintModelCacheService, ThreeDPrintProjectTracker.Engine.Services.PrintModelCacheService>();
            services.AddSingleton<IPrintModelImportService, ThreeDPrintProjectTracker.Engine.Services.PrintModelImportService>();
        }

        private void ConfigureTransients(IServiceCollection services)
        {
            services.AddTransient<ManagePrintersWindow>();
            services.AddTransient<ManagePrintersWindowViewModel>();

            services.AddTransient<ManageFilamentsWindow>();
            services.AddTransient<ManageFilamentsWindowViewModel>();
        }

        public static void SetTheme(bool useDark)
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