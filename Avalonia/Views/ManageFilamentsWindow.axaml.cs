using Avalonia.Controls;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;

namespace ThreeDPrintProjectTracker.Avalonia;

public partial class ManageFilamentsWindow : Window
{
    public ManageFilamentsWindow()
    {
        InitializeComponent();
    }

    public ManageFilamentsWindow(ManageFilamentsWindowViewModel vm) : this()
    {
        DataContext = vm;
    }
}