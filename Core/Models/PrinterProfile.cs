using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public record PrinterProfile
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }

        // ----------------------------
        // Standard layer settings
        // ----------------------------
        public double NozzleDiameter { get; init; }
        public double LayerHeight { get; init; }
        public double LineWidth { get; init; }

        // ----------------------------
        // First-layer settings
        // ----------------------------
        public double InitialLayerHeight { get; init; }          // e.g., 0.2mm
        public double InitialLayerLineWidth { get; init; }       // e.g., 0.45mm
        public double InitialLayerFlowGeneral { get; init; }    // multiplier e.g., 1.0
        public double InitialLayerFlowPerimeter { get; init; }  // multiplier for outer walls
        public double InitialLayerFlowInfill { get; init; }     // multiplier for infill

        // ----------------------------
        // Speeds (mm/s)
        // ----------------------------
        public double PrintSpeedGeneral { get; init; }
        public double PrintSpeedWall { get; init; }
        public double PrintSpeedInfill { get; init; }
        public double TravelSpeed { get; init; }

        public double InitialLayerPrintSpeedWall { get; init; }    // outer/inner wall first layer
        public double InitialLayerPrintSpeedInfill { get; init; }   // infill first layer
        public double InitialLayerTravelSpeed { get; init; }        // travel first layer
        public double InitialLayerPrintSpeedGeneral { get; init; }  // fallback

        // ----------------------------
        // Material flow multipliers
        // ----------------------------
        public double FlowPercentGeneral { get; init; }    // default 100%
        public double FlowPercentPerimeter { get; init; }  // outer/inner walls
        public double FlowPercentInfill { get; init; }     // infill

        // ----------------------------
        // Geometry
        // ----------------------------
        public int WallCount { get; init; }
        public double InfillDensity { get; init; }
        public bool SupportsEnabled { get; init; }

        // ----------------------------
        // Time adjustment / calibration
        // ----------------------------
        public double CalibrationFactor { get; init; }   // global fudge factor

        // ----------------------------
        // Travel and efficiency multipliers
        // ----------------------------
        public double TravelTimeFactor { get; init; }       // multiplies print time to estimate travel
        public double WallSpeedEfficiency { get; init; }    // slows perimeters for realistic speed
        public double InfillSpeedEfficiency { get; init; }  // slows infill for realistic speed
        public double TravelInefficiency { get; init; }     // accounts for Z-hops, retractions, detours
    }
}
