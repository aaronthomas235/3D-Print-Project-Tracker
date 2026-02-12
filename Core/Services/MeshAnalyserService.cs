using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.Models;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Services
{
    public class MeshAnalyserService : IMeshAnalyserService
    {
        public MeshAnalyserService()
        {

        }
        public async Task<MeshDimensions> AnalyseMesh(PrintModel model)
        {
            if (model?.Dimensions == null)
            {
                return new MeshDimensions(0,0,0);
            }

            return model.Dimensions;
        }
    }
}
