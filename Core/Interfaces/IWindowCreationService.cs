using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWindowCreationService
    {
        Task ShowManageFilamentsAsync();
        Task ShowManagePrintersAsync();
    }
}
