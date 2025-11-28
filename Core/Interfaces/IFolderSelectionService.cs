using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFolderSelectionService
    {
        Task<string?> SelectFolderAsync();
    }
}
