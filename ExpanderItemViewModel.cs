using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace _3DPrintProjectTracker
{
    public class ExpanderItemViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;

        public string Title { get; set; }
        public string Description { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        private bool _isProjectFile;

        public bool IsProjectFile
        {
            get => _isProjectFile;
            set
            {
                _isProjectFile = value;
                OnPropertyChanged(nameof(IsProjectFile));
            }
        }

        public ObservableCollection<ExpanderItemViewModel> Children { get; set; } = new();

        public ICommand ShowPartDetailsCommand { get; set; }

        public ExpanderItemViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            ShowPartDetailsCommand = new RelayCommand(ShowPartDetails, () => IsProjectFile);
        }

        private void ShowPartDetails()
        {
            _mainViewModel.ClickedExpanderItem = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
