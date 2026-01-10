using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public static class ReferencePrinterProfile
    {
        public static PrinterProfile Default => new()
        {
            Id = Guid.Empty,
            Name = "Reference 0.4mm FDM Printer",

            // ───── Hardware ─────
            NozzleDiameter = 0.4,

            // ───── Normal layers ─────
            LayerHeight = 0.2,
            LineWidth = 0.4,

            // ───── Initial layer ─────
            InitialLayerHeight = 0.2,
            InitialLayerLineWidth = 0.4,
            InitialLayerFlowGeneral = 1.0,
            InitialLayerFlowPerimeter = 1.0,
            InitialLayerFlowInfill = 1.0,

            // ───── Flow multipliers (percent, 100 = nominal) ─────
            FlowPercentGeneral = 100.0,
            FlowPercentPerimeter = 100.0,
            FlowPercentInfill = 100.0,

            // ───── Speeds (mm/s) ─────
            PrintSpeedGeneral = 50,
            PrintSpeedWall = 35,
            PrintSpeedInfill = 50,
            TravelSpeed = 150,

            InitialLayerPrintSpeedWall = 25,
            InitialLayerPrintSpeedInfill = 25,
            InitialLayerPrintSpeedGeneral = 25,
            InitialLayerTravelSpeed = 100,

            // ───── Inefficiency / calibration factors ─────
            WallSpeedEfficiency = 0.99,        // slightly slower than max
            InfillSpeedEfficiency = 0.99,      // slightly slower than max

            // ───── Geometry ─────
            WallCount = 2,
            InfillDensity = 0.2,

            // ───── Supports ─────
            SupportsEnabled = false,
            SupportDensity = 0.2,
            SupportVolumeFactor = 0.6,         // realistic support volume
            PrintSpeedSupport = 25,
            SupportSpeedEfficiency = 0.5,     // slow support printing
            SupportTravelFactor = 0.8,         // overhead for support travel

            // ───── Time calibration ─────
            TravelTimeFactor = 0.075,           // fraction of extrusion time added as travel
            CalibrationFactor = 0.95            // global fudge factor

        };
    }
}
