using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace _3DPrintProjectTracker
{
    public class ExpanderItemViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;

        private bool _isUpdatingChildren;

        public string Title { get; set; }
        public string Description { get; set; }

        private bool? _isChecked;
        public bool? IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                    OnIsCheckedChanged();
                }
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

            Children.CollectionChanged += (sender, eventArgs) =>
            {
                if (eventArgs.NewItems != null)
                {
                    foreach(ExpanderItemViewModel childExpanderItem in eventArgs.NewItems)
                    {
                        childExpanderItem.PropertyChanged += Child_PropertyChanged;
                    }
                }

                if (eventArgs.OldItems != null)
                {
                    foreach(ExpanderItemViewModel childExpanderItem in eventArgs.OldItems)
                    {
                        childExpanderItem.PropertyChanged -= Child_PropertyChanged;
                    }
                }
            };
        }

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (_isUpdatingChildren)
            {
                return;
            }
            if (eventArgs.PropertyName == nameof(IsChecked))
            {
                UpdateCheckStateFromChildren();
            }
        }

        private void UpdateCheckStateFromChildren()
        {
            if (IsProjectFile || Children.Count == 0)
            {
                return;
            }

            if (Children.All(child => child.IsChecked == true))
            {
                _isChecked = true;
            }
            else if (Children.All(child => child.IsChecked == false))
            {
                _isChecked = false;
            }
            else
            {
                _isChecked = null;
            }

            OnPropertyChanged(nameof(IsChecked));
        }

        private void OnIsCheckedChanged()
        {
            if (IsProjectFile)
            {
                if (IsChecked == null)
                {
                    IsChecked = false;
                }
                return;
            }

            if (IsChecked.HasValue)
            {
                _isUpdatingChildren = true;
                try
                {
                    foreach (var child in Children)
                        child.IsChecked = IsChecked;
                }
                finally
                {
                    _isUpdatingChildren = false;
                }
            }
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
