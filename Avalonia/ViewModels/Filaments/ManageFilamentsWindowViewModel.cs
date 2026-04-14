using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ThreeDPrintProjectTracker.Avalonia.ViewModels.Filaments;
using ThreeDPrintProjectTracker.Engine.Interfaces.Materials;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class ManageFilamentsWindowViewModel : ViewModelBase
    {
        private ISpoolService _spoolService;
        private IMaterialService _materialService;

        public ObservableCollection<SpoolItemViewModel> Spools { get; } = new();

        [ObservableProperty]
        private SpoolItemViewModel? selectedSpool;

        [ObservableProperty]
        private SpoolEditorViewModel? editor;

        public ManageFilamentsWindowViewModel(ISpoolService spoolService, IMaterialService materialService)
        {
            _spoolService = spoolService ?? throw new ArgumentNullException(nameof(spoolService));
            _materialService = materialService ?? throw new ArgumentNullException(nameof(materialService));

            foreach (var spool in _spoolService.GetAllSpools())
            {
                Spools.Add(new SpoolItemViewModel(spool));
            }

            SelectedSpool = Spools.FirstOrDefault();
        }

        [RelayCommand]
        private void NewSpool()
        {
            var record = DefaultSpools.Default with
            {
                Id = Guid.NewGuid(),
                Name = "New Spool"
            };

            _spoolService.AddSpool(record);

            var itemVm = new SpoolItemViewModel(record);
            Spools.Add(itemVm);

            SelectedSpool = itemVm;
        }

        [RelayCommand]
        private void DuplicateSpool()
        {
            if (SelectedSpool == null) return;

            var copyRecord = SelectedSpool.Model with
            {
                Id = Guid.NewGuid(),
                Name = SelectedSpool.Name + " (Copy)"
            };

            _spoolService.AddSpool(copyRecord);

            var itemVm = new SpoolItemViewModel(copyRecord);
            Spools.Add(itemVm);

            SelectedSpool = itemVm;
        }

        [RelayCommand]
        private void DeleteSpool()
        {
            if (SelectedSpool == null || SelectedSpool.Model.Id == Guid.Empty) return;

            if (_spoolService.RemoveSpool(SelectedSpool.Model.Id))
            {
                Spools.Remove(SelectedSpool);
                SelectedSpool = Spools.FirstOrDefault(s => s.Model.Id != Guid.Empty) ?? Spools.FirstOrDefault();
            }
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedSpool == null || Editor == null) return;

            Editor.ApplyChanges();
            _spoolService.UpdateSpool(Editor.Model);

            SelectedSpool.UpdateFromModel(Editor.Model);
        }

        partial void OnSelectedSpoolChanged(SpoolItemViewModel? value)
        {
            Editor = value == null ? null : new SpoolEditorViewModel(value.Model, _materialService.GetAllMaterials().ToList());
        }
    }
}
