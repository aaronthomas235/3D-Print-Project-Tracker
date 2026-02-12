using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IFileLauncherService
    {
        Task OpenFileAsync(string filePath);
    }
}
