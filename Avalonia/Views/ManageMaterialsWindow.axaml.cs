using Avalonia.Controls;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;

namespace ThreeDPrintProjectTracker.Avalonia;

public partial class ManageMaterialsWindow : Window
{
    public ManageMaterialsWindow()
    {
        InitializeComponent();
    }

    public ManageMaterialsWindow(ManageMaterialsWindowViewModel vm) : this()
    {
        DataContext = vm;
    }
}