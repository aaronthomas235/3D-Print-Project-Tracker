using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using AvaloniaApp.Services;
using AvaloniaApp.ViewModels;
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

            services.AddSingleton<IProjectTreeCoordinationService, Core.Services.ProjectTreeCoordinationService>();
            services.AddSingleton<IProjectTreeBuilderService, Core.Services.ProjectTreeBuilderService>();
            services.AddSingleton<IProjectTreeItemViewModelFactory, Core.Factories.ProjectTreeItemViewModelFactory>();
            services.AddSingleton<IFileManagementService, Core.Services.FileManagementService>();
            services.AddSingleton<ISupportedFileFormatsService, Core.Services.SupportedFileFormatsService>();
            services.AddSingleton<IPrinterProfileService, Core.Services.PrinterProfileService>();
            services.AddSingleton<IPrintTimeEstimationService, Core.Services.PrintTimeEstimationService>();
            services.AddSingleton<IMeshAnalyserService, Core.Services.MeshAnalyserService>();
            services.AddSingleton<IMaterialUsageEstimationService, Core.Services.MaterialUsageEstimationService>();
            services.AddSingleton<IPrintModelCacheService, Core.Services.PrintModelCacheService>();
            services.AddSingleton<IPrintModelImportService, Core.Services.PrintModelImportService>();
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