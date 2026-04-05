using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Models.Materials
{
    public record Spool
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required double OuterDiameterMm { get; init; }
        public required double HubDiameterMm { get; init; }
        public required double WidthMm { get; init; }
        public required double EmptyWeightGrams { get; init; }
        public required MaterialDefinition Material { get; set; }
        public double RemainingWeightGrams { get; set; }
    }
}
