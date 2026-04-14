using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Models.Materials
{
    public record MaterialDefinition
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required MaterialType MaterialType { get; init; }
        public double Diameter { get; init; }
        public required string Color { get; init; }
        public double Density { get; init; }
        public double Tolerance { get; init; }
    }

    public enum MaterialType
    {
        PLA,
        ABS,
        TPU
    }
}
