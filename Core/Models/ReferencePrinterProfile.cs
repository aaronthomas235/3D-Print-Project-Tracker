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
            WallSpeedEfficiency = 0.95,
            InfillSpeedEfficiency = 0.95,

            // ───── Geometry ─────
            WallCount = 2,
            InfillDensity = 0.2,
            SupportsEnabled = false,

            // ───── Time calibration ─────
            TravelTimeFactor = 0.1,
            CalibrationFactor = 0.85
        };
    }
}
