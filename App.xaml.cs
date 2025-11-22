using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using _3DPrintProjectTracker.Interfaces;
using _3DPrintProjectTracker.Services;
using _3DPrintProjectTracker.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace _3DPrintProjectTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IFileManagementService, FileManagementService>();
            services.AddSingleton<IFolderSelectionService, FolderSelectionService>();
            services.AddSingleton<MainViewModel>();
            services.AddTransient<MainWindow>();

            serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
