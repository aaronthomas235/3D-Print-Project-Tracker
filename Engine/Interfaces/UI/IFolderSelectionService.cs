using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces.UI
{
    public interface IFolderSelectionService
    {
        Task<string?> SelectFolderAsync();
    }
}
