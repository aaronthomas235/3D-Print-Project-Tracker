using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ThreeDPrintProjectTracker.Avalonia.ViewModels.Materials;

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