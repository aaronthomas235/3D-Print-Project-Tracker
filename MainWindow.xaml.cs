using _3DPrintProjectTracker;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace _3DPrintProjectTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
