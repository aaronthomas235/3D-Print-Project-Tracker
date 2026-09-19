using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces.UI
{
    public interface IFileLauncherService
    {
        Task OpenFileAsync(string filePath);
    }
}
