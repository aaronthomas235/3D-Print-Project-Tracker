using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public sealed class PrintModel
    {
        public double VolumeMm3 { get; init; }
        public double SurfaceAreaMm2 { get; init; }
        public double HeightMm { get; init; }
        public MeshDimensions? Dimensions { get; init; }
    }
}
