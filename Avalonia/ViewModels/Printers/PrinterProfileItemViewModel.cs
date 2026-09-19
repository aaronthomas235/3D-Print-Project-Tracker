using CommunityToolkit.Mvvm.ComponentModel;
using System;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels.Printers
{
    public partial class PrinterProfileItemViewModel : ObservableObject
    {
        public PrinterProfile Model { get; set; }

        public string Name => Model.Name;
        public double NozzleDiameter => Model.NozzleDiameter;


        public PrinterProfileItemViewModel(PrinterProfile model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void UpdateFromModel(PrinterProfile updated)
        {
            Model = updated ?? throw new ArgumentNullException(nameof(updated));
            OnPropertyChanged(string.Empty);
            
        }
    }
}
