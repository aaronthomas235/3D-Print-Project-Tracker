using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApp.ViewModels;

namespace AvaloniaApp;

public partial class ManageFilamentsWindow : Window
{
    public ManageFilamentsWindow(ManageFilamentsWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}