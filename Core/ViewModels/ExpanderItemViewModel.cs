using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Core.ViewModels
{
    public class ExpanderItemViewModel : ObservableObject, IDisposable
    {
        private IExpanderItemHost _expanderItemHost;

        private bool _isUpdatingChildren;
        private bool _isDisposed;


        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isProjectFile;
        public bool IsProjectFile
        {
            get => _isProjectFile;
            set
            {
                if (SetProperty(ref _isProjectFile, value))
                {
                    ShowPartDetailsCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool? _isChecked;
        public bool? IsChecked
        {
            get => _isChecked;
            set
            {
                if (SetProperty(ref _isChecked, value))
                {
                    OnIsCheckedChanged();
                }
            }
        }

        [JsonInclude]
        public ObservableCollection<ExpanderItemViewModel> Children { get; private set; } = new();

        [JsonIgnore]
        public IRelayCommand ShowPartDetailsCommand { get; private set; }

        public ExpanderItemViewModel()
        {
            InitialiseCommonResources(_expanderItemHost);
        }

        public ExpanderItemViewModel(IExpanderItemHost expanderItemHost)
        {
            _expanderItemHost = expanderItemHost ?? throw new ArgumentNullException(nameof(expanderItemHost));
            InitialiseCommonResources(expanderItemHost);
        }

        private void InitialiseCommonResources(IExpanderItemHost expanderItemHost)
        {
            _expanderItemHost = expanderItemHost;

            ShowPartDetailsCommand = new RelayCommand(
            execute: ShowPartDetails,
            canExecute: () => IsProjectFile
            );

            Children.CollectionChanged += OnChildrenCollectionChanged;
            foreach(var child in Children)
            {
                child.InitialiseCommonResources(expanderItemHost);
            }
        }

        public void InitialiseRuntimeResources(IExpanderItemHost expanderItemHost)
        {
            InitialiseCommonResources(expanderItemHost);
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.NewItems != null)
            {
                foreach (ExpanderItemViewModel child in eventArgs.NewItems)
                {
                    child.PropertyChanged += OnChildPropertyChanged;
                }
            }

            if (eventArgs.OldItems != null)
            {
                foreach (ExpanderItemViewModel child in eventArgs.OldItems)
                {
                    child.PropertyChanged -= OnChildPropertyChanged;
                }
            }
        }

        private void OnChildPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (_isUpdatingChildren || eventArgs.PropertyName != nameof(IsChecked))
            {
                return;
            }

            UpdateCheckStateFromChildren();
        }

        private void UpdateCheckStateFromChildren()
        {
            if (IsProjectFile || Children.Count == 0)
            {
                return;
            }

            bool? newCheckState = CalculateCheckStateFromChildren();

            if (_isChecked != newCheckState)
            {
                SetProperty(ref _isChecked, newCheckState, nameof(IsChecked));
            }
        }

        private bool? CalculateCheckStateFromChildren()
        {
            if (Children.All(c => c.IsChecked == true))
            {
                return true;
            }
            if (Children.All(c => c.IsChecked == false))
            {
                return false;
            }
            return null;
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
                    foreach(var child in Children)
                    {
                        child.IsChecked = IsChecked;
                    }
                }
                finally
                {
                    _isUpdatingChildren = false;
                }
            }
        }

        private void ShowPartDetails()
        {
            _expanderItemHost.OnExpanderItemSelected(this);
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            foreach (var child in Children)
                child.PropertyChanged -= OnChildPropertyChanged;

            Children.CollectionChanged -= OnChildrenCollectionChanged;
        }
    }
}
