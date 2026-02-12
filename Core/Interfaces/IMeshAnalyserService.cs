using ThreeDPrintProjectTracker.Engine.Models;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IMeshAnalyserService
    {
        Task<MeshDimensions> AnalyseMesh(PrintModel model);
    }
}
