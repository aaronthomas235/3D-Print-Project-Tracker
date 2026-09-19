using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Avalonia.Interfaces;

namespace ThreeDPrintProjectTracker.Avalonia.Services
{
    public class FolderSelectionService : IFolderSelectionService
    {
        public async Task<string?> SelectFolderAsync(Window parent)
        {
            var folderSelectionOptions = new FolderPickerOpenOptions
            {
                Title = "Select your projects folder",
                AllowMultiple = false
            };

            var result = await parent.StorageProvider.OpenFolderPickerAsync(folderSelectionOptions);

            return result.Count > 0
                ? result[0].Path.LocalPath
                : null;
        }
    }
}
