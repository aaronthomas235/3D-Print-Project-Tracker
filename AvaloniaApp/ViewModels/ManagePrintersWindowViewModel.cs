using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.Models;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class ManagePrintersWindowViewModel : ViewModelBase
    {
        private IPrinterProfileService _printerProfileService;

        public ObservableCollection<PrinterProfileItemViewModel> Profiles { get; } = new();

        [ObservableProperty]
        private PrinterProfileItemViewModel? selectedProfile;

        [ObservableProperty]
        private PrinterProfileEditorViewModel? editor;

        public ManagePrintersWindowViewModel(IPrinterProfileService printerProfileService)
        {
            _printerProfileService = printerProfileService ?? throw new ArgumentNullException(nameof(printerProfileService));

            foreach (var profile in _printerProfileService.GetAllPrinterProfiles())
            {
                Profiles.Add(new PrinterProfileItemViewModel(profile));
            }

            SelectedProfile = Profiles.FirstOrDefault();
        }


        [RelayCommand]
        private void NewProfile()
        {
            var record = ReferencePrinterProfile.Default with
            {
                Id = Guid.NewGuid(),
                Name = "New Printer"
            };

            _printerProfileService.AddProfile(record);

            var itemVm = new PrinterProfileItemViewModel(record);
            Profiles.Add(itemVm);

            SelectedProfile = itemVm; // now the type matches
        }


        [RelayCommand]
        private void DuplicateProfile()
        {
            if (SelectedProfile == null) return;

            var copyRecord = SelectedProfile.Model with
            {
                Id = Guid.NewGuid(),
                Name = SelectedProfile.Name + " (Copy)"
            };

            _printerProfileService.AddProfile(copyRecord);

            var itemVm = new PrinterProfileItemViewModel(copyRecord);
            Profiles.Add(itemVm);

            SelectedProfile = itemVm;
        }


        [RelayCommand]
        private void DeleteProfile()
        {
            if (SelectedProfile == null || SelectedProfile.Model.Id == Guid.Empty) return;

            if (_printerProfileService.RemoveProfile(SelectedProfile.Model.Id))
            {
                Profiles.Remove(SelectedProfile);
                SelectedProfile = Profiles.FirstOrDefault(p => p.Model.Id != Guid.Empty) ?? Profiles.FirstOrDefault();
            }
        }


        [RelayCommand]
        private void Save()
        {
            if (SelectedProfile == null || Editor == null) return;

            var updatedRecord = Editor.ApplyChanges();
            _printerProfileService.UpdateProfile(updatedRecord);

            // Update the item VM with the new record
            SelectedProfile.UpdateFromModel(updatedRecord);
        }



        partial void OnSelectedProfileChanged(PrinterProfileItemViewModel? value)
        {
            Editor = value == null ? null : new PrinterProfileEditorViewModel(value.Model);
        }


    }
}
