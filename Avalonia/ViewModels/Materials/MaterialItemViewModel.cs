using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels.Materials
{
    public class MaterialItemViewModel : ObservableObject
    {
        public MaterialDefinition Model {  get; private set; }

        public string Name => Model.Name;
        public string MaterialType => Model.MaterialType.ToString();
        public double Diameter => Model.Diameter;
        public double Density => Model.Density;
        public double Tolerance => Model.Tolerance;

        public MaterialItemViewModel(MaterialDefinition model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void UpdateFromModel(MaterialDefinition updatedModel)
        {
            Model = updatedModel ?? throw new ArgumentNullException(nameof(updatedModel));
            OnPropertyChanged(string.Empty);
        }

    }
}
