using Avalonia.Controls;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Avalonia.Interfaces
{
    public interface IFolderSelectionService
    {
        Task<string?> SelectFolderAsync(Window parent);
    }
}
