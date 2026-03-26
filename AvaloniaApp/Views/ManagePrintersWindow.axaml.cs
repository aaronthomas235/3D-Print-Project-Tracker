using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;

namespace ThreeDPrintProjectTracker.Avalonia;

public partial class ManagePrintersWindow : Window
{
    public ManagePrintersWindow()
    {
        InitializeComponent();
    }

    public ManagePrintersWindow(ManagePrintersWindowViewModel vm)
        : this()
    {
        DataContext = vm;
    }
}