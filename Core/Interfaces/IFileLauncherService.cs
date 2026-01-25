using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileLauncherService
    {
        Task OpenFileAsync(string filePath);
    }
}
