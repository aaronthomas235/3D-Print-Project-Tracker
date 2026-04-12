using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Models.Materials
{
    public class DefaultMaterialCatalog
    {
        public static MaterialDefinition Pla => new()
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Name = "Generic PLA",
            MaterialType = "PLA",
            Diameter = 1.75,
            Density = 1.24,
            Tolerance = 0.02,
            Color = "Generic"
        };

        public static MaterialDefinition Abs => new()
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Name = "Generic ABS",
            MaterialType = "ABS",
            Diameter = 1.75,
            Density = 1.04,
            Tolerance = 0.03,
            Color = "Generic"
        };

        public static MaterialDefinition Tpu => new()
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Name = "Generic TPU",
            MaterialType = "TPU",
            Diameter = 1.75,
            Density = 1.20,
            Tolerance = 0.05,
            Color = "Generic"
        };
    }
}
