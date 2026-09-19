using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels.Materials
{
    public partial class MaterialEditorViewModel : ObservableObject
    {
        [ObservableProperty] private MaterialDefinition _model;

        public IReadOnlyList<MaterialType> MaterialTypes { get; } = Enum.GetValues<MaterialType>();

        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private MaterialType materialType;
        [ObservableProperty] private double diameter;
        [ObservableProperty] private double density;
        [ObservableProperty] private double tolerance;

        public MaterialEditorViewModel(MaterialDefinition model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            LoadFromModel(model);

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName != nameof(HasChanges))
                    OnPropertyChanged(nameof(HasChanges));
            };
        }

        private void LoadFromModel(MaterialDefinition model)
        {
            Name = model.Name;
            MaterialType = model.MaterialType;
            Diameter = model.Diameter;
            Density = model.Density;
            Tolerance = model.Tolerance;
        }

        public bool HasChanges =>
            Name != Model.Name ||
            MaterialType != Model.MaterialType ||
            Diameter != Model.Diameter ||
            Density != Model.Density ||
            Tolerance != Model.Tolerance;

        public void ApplyChanges()
        {
            Model = Model with
            {
                Name = Name,
                MaterialType = MaterialType,
                Diameter = Diameter,
                Density = Density,
                Tolerance = Tolerance
            };
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName != nameof(HasChanges))
            {
                base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasChanges)));
            }
        }

    }
}
