using Core.ViewModels;

namespace Core.Interfaces
{
    public interface IExpanderItemHost
    {
        void OnExpanderItemSelected(ExpanderItemViewModel item);
    }
}
