using System;

namespace ThreeDPrintProjectTracker.Engine.Models
{
    public record PrinterProfile
    {
        public Guid Id { get; init; }
        public required string Name { get; set; }

        // ───── Hardware ─────
        public double NozzleDiameter { get; set; }

        // ───── Normal layers ─────
        public double LayerHeight { get; set; }
        public double LineWidth { get; set; }

        // ───── Initial layer ─────
        public double InitialLayerHeight { get; set; }
        public double InitialLayerLineWidth { get; set; }

        public double InitialLayerFlowGeneral { get; set; }
        public double InitialLayerFlowPerimeter { get; set; }
        public double InitialLayerFlowInfill { get; set; }

        // ───── Flow multipliers (percent, 100 = nominal) ─────
        public double FlowPercentGeneral { get; set; }
        public double FlowPercentPerimeter { get; set; }
        public double FlowPercentInfill { get; set; }

        // ───── Speeds (mm/s) ─────
        public double PrintSpeedGeneral { get; set; }
        public double PrintSpeedWall { get; set; }
        public double PrintSpeedInfill { get; set; }
        public double TravelSpeed { get; set; }

        public double InitialLayerPrintSpeedWall { get; set; }
        public double InitialLayerPrintSpeedInfill { get; set; }
        public double InitialLayerPrintSpeedGeneral { get; set; }
        public double InitialLayerTravelSpeed { get; set; }

        // ───── Inefficiency / calibration factors ─────
        public double WallSpeedEfficiency { get; set; }
        public double InfillSpeedEfficiency { get; set; }

        // ───── Geometry ─────
        public int WallCount { get; set; }
        public double InfillDensity { get; set; }

        // ───── Supports ─────
        public bool SupportsEnabled { get; set; }
        public double SupportDensity { get; set; }
        public double SupportVolumeFactor { get; set; }
        public double PrintSpeedSupport { get; set; }
        public double SupportSpeedEfficiency { get; set; }
        public double SupportTravelFactor { get; set; }

        // ───── Time calibration ─────
        public double TravelTimeFactor { get; set; }
        public double CalibrationFactor { get; set; }
    }
}
