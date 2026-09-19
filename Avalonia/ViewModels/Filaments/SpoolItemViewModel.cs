using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using ThreeDPrintProjectTracker.Engine.Models.Materials;
using ThreeDPrintProjectTracker.Avalonia.Services;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels.Filaments
{
    public partial class SpoolItemViewModel : ObservableObject
    {
        public Spool Model { get; set; }

        public string Name => Model.Name;
        public string MaterialName => Model.Material.Name;
        public MaterialType MaterialType => Model.Material.MaterialType;
        public Color Colour => Model.Colour.ToAvalonia();

        public double RemainingWeightGrams => Model.RemainingWeightGrams;

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
