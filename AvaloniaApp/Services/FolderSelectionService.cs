using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ThreeDPrintProjectTracker.Engine.Interfaces;

namespace ThreeDPrintProjectTracker.Avalonia.Services
{
    public class FolderSelectionService : IFolderSelectionService
    {
        private readonly Window _windowInstance;

        public FolderSelectionService(Window window)
        {
            _windowInstance = window;
        }

        public async Task<string?> SelectFolderAsync()
        {
            var folderSelectionOptions = new FolderPickerOpenOptions
            {
                Title = "Select your projects folder",
                AllowMultiple = false
            };

            var selectionResult = await _windowInstance.StorageProvider.OpenFolderPickerAsync(folderSelectionOptions);
            return selectionResult.Count >0 ? selectionResult[0].Path.LocalPath : null;
        }
    }
}
