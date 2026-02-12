using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ThreeDPrintProjectTracker.Engine.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Avalonia.Services
{
    public class WindowCreationService : IWindowCreationService
    {
        private readonly IServiceProvider _services;

        public WindowCreationService(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        private Window GetMainWindow()
        {
            return (Application.Current!.ApplicationLifetime
                as IClassicDesktopStyleApplicationLifetime)!
                .MainWindow!;
        }

        public async Task ShowManagePrintersAsync()
        {
            var window = _services.GetRequiredService<ManagePrintersWindow>();
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            await window.ShowDialog(GetMainWindow());
        }

        public async Task ShowManageFilamentsAsync()
        {
            var window = _services.GetRequiredService<ManageFilamentsWindow>();
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            await window.ShowDialog(GetMainWindow());
        }
    }
}
