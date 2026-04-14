using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class SpoolEditorViewModel :ObservableObject
    {
        [ObservableProperty]
        private Spool _model;

        // ───── General ─────
        [ObservableProperty] private string name = string.Empty;

        // ───── Physical ─────
        [ObservableProperty] private double outerDiameterMm;
        [ObservableProperty] private double hubDiameterMm;
        [ObservableProperty] private double widthMm;
        [ObservableProperty] private double emptyWeightGrams;

        // ───── Usage ─────
        [ObservableProperty] private double remainingWeightGrams;

        // ───── Material ─────
        [ObservableProperty] private MaterialDefinition? selectedMaterial;

        public IReadOnlyList<MaterialDefinition> Materials { get; }

        public SpoolEditorViewModel(Spool model, IReadOnlyList<MaterialDefinition> materials)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Materials = materials;

            LoadFromModel(model);

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName != nameof(HasChanges))
                    OnPropertyChanged(nameof(HasChanges));
            };
        }

        private void LoadFromModel(Spool model)
        {
            Name = model.Name;

            OuterDiameterMm = model.OuterDiameterMm;
            HubDiameterMm = model.HubDiameterMm;
            WidthMm = model.WidthMm;
            EmptyWeightGrams = model.EmptyWeightGrams;

            RemainingWeightGrams = model.RemainingWeightGrams;

            SelectedMaterial = model.Material;
        }

        public bool HasChanges =>
            Name != Model.Name ||
            OuterDiameterMm != Model.OuterDiameterMm ||
            HubDiameterMm != Model.HubDiameterMm ||
            WidthMm != Model.WidthMm ||
            EmptyWeightGrams != Model.EmptyWeightGrams ||
            RemainingWeightGrams != Model.RemainingWeightGrams ||
            SelectedMaterial != Model.Material;

        public void ApplyChanges()
        {
            Model = Model with
            {
                Name = Name,
                OuterDiameterMm = OuterDiameterMm,
                HubDiameterMm = HubDiameterMm,
                WidthMm = WidthMm,
                EmptyWeightGrams = EmptyWeightGrams,
                RemainingWeightGrams = RemainingWeightGrams,
                Material = SelectedMaterial!
            };
        }
    }
}
