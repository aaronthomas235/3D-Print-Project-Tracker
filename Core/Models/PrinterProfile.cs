using System;

namespace Core.Models
{
    public record PrinterProfile
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }

        // ───── Hardware ─────
        public double NozzleDiameter { get; init; }

        // ───── Normal layers ─────
        public double LayerHeight { get; init; }
        public double LineWidth { get; init; }

        // ───── Initial layer ─────
        public double InitialLayerHeight { get; init; }
        public double InitialLayerLineWidth { get; init; }

        public double InitialLayerFlowGeneral { get; init; }
        public double InitialLayerFlowPerimeter { get; init; }
        public double InitialLayerFlowInfill { get; init; }

        // ───── Flow multipliers (percent, 100 = nominal) ─────
        public double FlowPercentGeneral { get; init; }
        public double FlowPercentPerimeter { get; init; }
        public double FlowPercentInfill { get; init; }

        // ───── Speeds (mm/s) ─────
        public double PrintSpeedGeneral { get; init; }
        public double PrintSpeedWall { get; init; }
        public double PrintSpeedInfill { get; init; }
        public double TravelSpeed { get; init; }

        public double InitialLayerPrintSpeedWall { get; init; }
        public double InitialLayerPrintSpeedInfill { get; init; }
        public double InitialLayerPrintSpeedGeneral { get; init; }
        public double InitialLayerTravelSpeed { get; init; }

        // ───── Inefficiency / calibration factors ─────
        public double WallSpeedEfficiency { get; init; }
        public double InfillSpeedEfficiency { get; init; }

        // ───── Geometry ─────
        public int WallCount { get; init; }
        public double InfillDensity { get; init; }

        // ───── Supports ─────
        public bool SupportsEnabled { get; init; }
        public double SupportDensity { get; init; }
        public double SupportVolumeFactor { get; init; }
        public double PrintSpeedSupport { get; init; }
        public double SupportSpeedEfficiency { get; init; }
        public double SupportTravelFactor { get; init; }

        // ───── Time calibration ─────
        public double TravelTimeFactor { get; init; }
        public double CalibrationFactor { get; init; }
    }
}
