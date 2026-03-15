using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;

namespace ThreeDPrintProjectTracker.Avalonia;

public partial class ManageFilamentsWindow : Window
{
    // Required for XAML loader + designer
    public ManageFilamentsWindow()
    {
        InitializeComponent();
    }

    // Used by DI
    public ManageFilamentsWindow(ManageFilamentsWindowViewModel vm)
        : this()
    {
        DataContext = vm;
    }
}