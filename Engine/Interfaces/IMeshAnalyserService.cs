using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Printing;
using ThreeDPrintProjectTracker.Engine.Models.Geometry;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IMeshAnalyserService
    {
        Task<MeshDimensions> AnalyseMesh(PrintModel model);
    }
}
