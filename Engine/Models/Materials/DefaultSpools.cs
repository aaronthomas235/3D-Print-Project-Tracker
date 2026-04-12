using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Models.Materials
{
    public class DefaultSpools
    {
        public static Spool Default => new()
        {
            Id = Guid.Empty,
            Name = "New Spool",
            OuterDiameterMm = 200,
            HubDiameterMm = 52,
            WidthMm = 60,
            EmptyWeightGrams = 250,
            RemainingWeightGrams = 1000,
            Material = DefaultMaterialCatalog.Pla
        };
    }
}
