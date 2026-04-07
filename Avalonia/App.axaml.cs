using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ThreeDPrintProjectTracker.Avalonia.Services;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;
using ThreeDPrintProjectTracker.Avalonia.Views;
using ThreeDPrintProjectTracker.Engine.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using ThreeDPrintProjectTracker.Engine.Services.Projects;
using ThreeDPrintProjectTracker.Engine.Services.Printing;
using ThreeDPrintProjectTracker.Engine.Services.Models;
using ThreeDPrintProjectTracker.Engine.Services.Infrastructure;
using ThreeDPrintProjectTracker.Engine.Interfaces.Printing;
using ThreeDPrintProjectTracker.Engine.Interfaces.Projects;
using ThreeDPrintProjectTracker.Engine.Interfaces.Models;
using ThreeDPrintProjectTracker.Engine.Interfaces.Infrastructure;
using ThreeDPrintProjectTracker.Engine.Interfaces.UI;
using ThreeDPrintProjectTracker.Avalonia.Interfaces;

namespace ThreeDPrintProjectTracker.Avalonia
{
    public partial class App : Application
    {
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

                IServiceProvider serviceProvider = services.BuildServiceProvider();

                desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<IWindowCreationService, WindowCreationService>();
            services.AddSingleton<IFileLauncherService, FileLauncherService>();
            services.AddSingleton<IThemeChangerService, ThemeChangerService>();

            services.AddSingleton<IFolderSelectionService>(sp => new FolderSelectionService(sp.GetRequiredService<MainWindow>()));

            services.AddSingleton<IProjectTreeCoordinationService, Engine.Services.ProjectTreeCoordinationService>();
            services.AddSingleton<IProjectTreeBuilderService, Engine.Services.ProjectTreeBuilderService>();
            services.AddSingleton<IProjectTreeItemViewModelFactory, Factories.ProjectTreeItemViewModelFactory>();
            services.AddSingleton<IFileManagementService, FileManagementService>();
            services.AddSingleton<ISupportedFileFormatsService, Engine.Services.SupportedFileFormatsService>();
            services.AddSingleton<IPrinterProfileService, PrinterProfileService>();
            services.AddSingleton<IPrintTimeEstimationService, Engine.Services.PrintTimeEstimationService>();
            services.AddSingleton<IMeshAnalyserService, MeshAnalyserService>();
            services.AddSingleton<IMaterialUsageEstimationService, MaterialUsageEstimationService>();
            services.AddSingleton<IPrintModelCacheService, PrintModelCacheService>();
            services.AddSingleton<IPrintModelImportService, Engine.Services.PrintModelImportService>();
        }

        private void ConfigureTransients(IServiceCollection services)
        {
            services.AddTransient<ManagePrintersWindow>();
            services.AddTransient<ManagePrintersWindowViewModel>();

            services.AddTransient<ManageFilamentsWindow>();
            services.AddTransient<ManageFilamentsWindowViewModel>();
        }
    }
}