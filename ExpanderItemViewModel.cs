using System.ComponentModel;

namespace _3DPrintProjectTracker
{
    public class ExpanderItemViewModel : INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
