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
        private Spool _model = default!;

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
            Materials = materials ?? throw new ArgumentNullException(nameof(model));

            LoadFromModel(model);
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
            SelectedMaterial?.Id != Model.Material.Id;

        public double RemainingPercentage => (RemainingWeightGrams + EmptyWeightGrams) == 0
            ? 0
            : RemainingWeightGrams / (RemainingWeightGrams + EmptyWeightGrams);

        public void ApplyChanges()
        {
            if (SelectedMaterial == null)
            {
                throw new InvalidOperationException("Material must be selected.");
            }

            Model = Model with
            {
                Name = Name,
                OuterDiameterMm = OuterDiameterMm,
                HubDiameterMm = HubDiameterMm,
                WidthMm = WidthMm,
                EmptyWeightGrams = EmptyWeightGrams,
                RemainingWeightGrams = RemainingWeightGrams,
                Material = SelectedMaterial
            };
        }

        private void NotifyChanged()
        {
            OnPropertyChanged(nameof(HasChanges));
        }

        partial void OnNameChanged(string value) => NotifyChanged();
        partial void OnOuterDiameterMmChanged(double value) => NotifyChanged();
        partial void OnHubDiameterMmChanged(double value) => NotifyChanged();
        partial void OnWidthMmChanged(double value) => NotifyChanged();
        partial void OnEmptyWeightGramsChanged(double value) => NotifyChanged();
        partial void OnRemainingWeightGramsChanged(double value)
        {
            var rounded = Math.Round(value);

            if (rounded != value)
                RemainingWeightGrams = rounded;

            NotifyChanged();
        }
        partial void OnSelectedMaterialChanged(MaterialDefinition? value) => NotifyChanged();
    }
}
