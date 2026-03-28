using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IFolderSelectionService
    {
        Task<string?> SelectFolderAsync();
    }
}
