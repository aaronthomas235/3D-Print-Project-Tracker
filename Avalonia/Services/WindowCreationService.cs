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
        private readonly IClassicDesktopStyleApplicationLifetime _lifetime;

        public WindowCreationService(IServiceProvider services, IClassicDesktopStyleApplicationLifetime lifetime)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        }

        private Window GetMainWindow()
        {
            return _lifetime.MainWindow!;
        }

        private async Task ShowDialogAsync<TWindow>() where TWindow : Window
        {
            var window = _services.GetRequiredService<TWindow>();
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            await window.ShowDialog(GetMainWindow());
        }

        public async Task ShowManagePrintersAsync() => await ShowDialogAsync<ManagePrintersWindow>();

        public async Task ShowManageFilamentsAsync() => await ShowDialogAsync<ManageFilamentsWindow>();
    }
}
