using CommunityToolkit.Mvvm.ComponentModel;
using System;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class SpoolItemViewModel : ObservableObject
    {
        public Spool Model { get; set; }

        public string Name => Model.Name;
        public string MaterialName => Model.Material.Name;
        public double RemainingWeight => Model.RemainingWeightGrams;

        public SpoolItemViewModel(Spool model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
        
        public void UpdateFromModel(Spool updated)
        {
            Model = updated ?? throw new ArgumentNullException(nameof(updated));
            OnPropertyChanged(string.Empty);
        }
    }
}
