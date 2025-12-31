using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AvaloniaApp.Services
{
    public sealed class FileLauncherService : IFileLauncherService
    {
        private readonly Dictionary<Func<bool>, Action<string>> _platformLaunchers;

        public FileLauncherService()
        {
            _platformLaunchers = new Dictionary<Func<bool>, Action<string>>
        {
            { () => OperatingSystem.IsWindows(), path => OpenWindows(path) },
            { () => OperatingSystem.IsMacOS(), path => OpenMac(path) },
            { () => OperatingSystem.IsLinux(), path => OpenLinux(path) }
        };
        }

        public Task OpenFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return Task.CompletedTask;
            }

            foreach(var platformLauncher in _platformLaunchers)
            {
                if (platformLauncher.Key())
                {
                    platformLauncher.Value(filePath);
                    return Task.CompletedTask;
                }
            }

            throw new PlatformNotSupportedException("Unsupported OS for file launching.");
        }

        #region Platform-specific helpers
        private static void OpenWindows(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        private static void OpenMac(string path)
        {
            Process.Start("open", $"\"{path}\"");
        }

        private static void OpenLinux(string path)
        {
            Process.Start("xdg-open", $"\"{path}\"");
        }
        #endregion
    }
}
