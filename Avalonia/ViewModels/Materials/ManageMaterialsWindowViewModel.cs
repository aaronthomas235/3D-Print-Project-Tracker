using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ThreeDPrintProjectTracker.Avalonia.ViewModels.Materials;
using ThreeDPrintProjectTracker.Engine.Interfaces.Materials;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class ManageMaterialsWindowViewModel : ViewModelBase
    {
        private IMaterialService _materialService;

        public ObservableCollection<MaterialItemViewModel> Materials { get; } = new();

        [ObservableProperty] private MaterialItemViewModel? selectedMaterial;

        [ObservableProperty] private MaterialEditorViewModel? editor;

        public ManageMaterialsWindowViewModel(IMaterialService materialService)
        {
            _materialService = materialService ?? throw new ArgumentNullException(nameof(materialService));

            foreach(var material in _materialService.GetAllMaterials())
            {
                Materials.Add(new MaterialItemViewModel(material));
            }

            SelectedMaterial = Materials.FirstOrDefault();
        }

        [RelayCommand]
        private void NewPrintingMaterial()
        {
            var record = DefaultMaterialCatalog.NewMaterial with
            {
                Id = Guid.NewGuid()
            };

            _materialService.AddMaterial(record);

            var itemVm = new MaterialItemViewModel(record);
            Materials.Add(itemVm);

            SelectedMaterial = itemVm;
        }

        [RelayCommand]
        private void DuplicatePrintingMaterial()
        {
            if (SelectedMaterial == null) return;

            var copyRecord = SelectedMaterial.Model with
            {
                Id = Guid.NewGuid(),
                Name = SelectedMaterial.Name + " (Copy)"
            };

            _materialService.AddMaterial(copyRecord);

            var itemVm = new MaterialItemViewModel(copyRecord);
            Materials.Add(itemVm);

            SelectedMaterial = itemVm;
        }

        [RelayCommand]
        private void DeletePrintingMaterial()
        {
            if (SelectedMaterial == null || SelectedMaterial.Model.Id == Guid.Empty) return;

            if (_materialService.RemoveMaterial(SelectedMaterial.Model.Id))
            {
                Materials.Remove(SelectedMaterial);
                SelectedMaterial = Materials.FirstOrDefault(m => m.Model.Id != Guid.Empty) ?? Materials.FirstOrDefault();
            }
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedMaterial == null || Editor == null) return;

            Editor.ApplyChanges();
            _materialService.UpdateMaterial(Editor.Model);

            SelectedMaterial.UpdateFromModel(Editor.Model);
        }

        partial void OnSelectedMaterialChanged(MaterialItemViewModel? value)
        {
            Editor = value == null ? null : new MaterialEditorViewModel(value.Model);
        }
    }
}
