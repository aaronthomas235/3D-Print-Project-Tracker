using Avalonia.Controls;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;

namespace ThreeDPrintProjectTracker.Avalonia;

public partial class ManageFilamentsWindow : Window
{
    public ManageFilamentsWindow(ManageFilamentsWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}