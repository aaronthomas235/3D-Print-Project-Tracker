using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class PrinterProfileEditorViewModel : ObservableObject
    {
        private readonly PrinterProfile _model;

        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private double nozzleDiameter = double.MinValue;
        [ObservableProperty] private double layerHeight;

        public PrinterProfileEditorViewModel(PrinterProfile model)
        {
            _model = model;

            name = model.Name;
            nozzleDiameter = model.NozzleDiameter;
            layerHeight = model.LayerHeight;

            // Update HasChanges when any property changes
            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName is not nameof(HasChanges))
                    OnPropertyChanged(nameof(HasChanges));
            };
        }

        public bool HasChanges =>
            Name != _model.Name ||
            NozzleDiameter != _model.NozzleDiameter ||
            LayerHeight != _model.LayerHeight;

        public PrinterProfile ApplyChanges()
        {
            return _model with
            {
                Name = Name,
                NozzleDiameter = NozzleDiameter,
                LayerHeight = LayerHeight
            };
        }
    }

}
