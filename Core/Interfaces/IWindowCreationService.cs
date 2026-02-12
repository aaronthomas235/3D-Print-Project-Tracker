using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IWindowCreationService
    {
        Task ShowManageFilamentsAsync();
        Task ShowManagePrintersAsync();
    }
}
