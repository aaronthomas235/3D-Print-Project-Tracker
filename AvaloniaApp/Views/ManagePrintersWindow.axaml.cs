using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApp.ViewModels;

namespace AvaloniaApp;

public partial class ManagePrintersWindow : Window
{
    public ManagePrintersWindow(ManagePrintersWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}