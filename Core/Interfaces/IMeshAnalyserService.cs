using Core.Models;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMeshAnalyserService
    {
        Task<MeshDimensions> AnalyseMesh(PrintModel model);
    }
}
