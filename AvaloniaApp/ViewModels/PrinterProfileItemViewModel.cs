using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ThreeDPrintProjectTracker.Engine.Models;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class PrinterProfileItemViewModel : ObservableObject
    {
        public PrinterProfile Model { get; private set; }

        [ObservableProperty] private string name;

        [ObservableProperty] private double nozzleDiameter;

        [ObservableProperty] private double layerHeight;
        [ObservableProperty] private double lineWidth;

        // ───── Initial layer ─────

        [ObservableProperty] private double initialLayerHeight;
        [ObservableProperty] private double initialLayerLineWidth;

        [ObservableProperty] private double initialLayerFlowGeneral;
        [ObservableProperty] private double initialLayerFlowPerimeter;
        [ObservableProperty] private double initialLayerFlowInfill;

        // ───── Flow multipliers  ─────
        [ObservableProperty] private double flowPercentGeneral;
        [ObservableProperty] private double flowPercentPerimeter;
        [ObservableProperty] private double flowPercentInfill;

        // ───── Speeds (mm/s) ─────
        [ObservableProperty] private double printSpeedGeneral;
        [ObservableProperty] private double printSpeedWall;
        [ObservableProperty] private double printSpeedInfill;
        [ObservableProperty] private double travelSpeed;

        [ObservableProperty] private double initialLayerPrintSpeedWall;
        [ObservableProperty] private double initialLayerPrintSpeedInfill;
        [ObservableProperty] private double initialLayerPrintSpeedGeneral;
        [ObservableProperty] private double initialLayerTravelSpeed;

        // ───── Inefficiency / calibration factors ─────
        [ObservableProperty] private double wallSpeedEfficiency;
        [ObservableProperty] private double infillSpeedEfficiency;

        // ───── Geometry ─────
        [ObservableProperty] private int wallCount;
        [ObservableProperty] private double infillDensity;

        // ───── Supports ─────
        [ObservableProperty] private bool supportsEnabled;
        [ObservableProperty] private double supportDensity;
        [ObservableProperty] private double supportVolumeFactor;
        [ObservableProperty] private double printSpeedSupport;
        [ObservableProperty] private double supportSpeedEfficiency;
        [ObservableProperty] private double supportTravelFactor;

        // ───── Time calibration ─────
        [ObservableProperty] private double travelTimeFactor;
        [ObservableProperty] private double calibrationFactor;


        public PrinterProfileItemViewModel(PrinterProfile model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            name = model.Name;
            nozzleDiameter = model.NozzleDiameter;
            LayerHeight = model.LayerHeight;
            LineWidth = model.LineWidth;
            InitialLayerHeight = model.InitialLayerHeight;
            InitialLayerLineWidth = model.InitialLayerLineWidth;
            InitialLayerFlowGeneral = model.InitialLayerFlowGeneral;
            InitialLayerFlowPerimeter = model.InitialLayerFlowPerimeter;
            InitialLayerFlowInfill = model.InitialLayerFlowInfill;
            FlowPercentGeneral = model.FlowPercentGeneral;
            FlowPercentPerimeter = model.FlowPercentPerimeter;
            FlowPercentInfill = model.FlowPercentInfill;
            PrintSpeedGeneral = model.PrintSpeedGeneral;
            PrintSpeedWall = model.PrintSpeedWall;
            PrintSpeedInfill = model.PrintSpeedInfill;
            TravelSpeed = model.TravelSpeed;
            InitialLayerPrintSpeedWall = model.InitialLayerPrintSpeedWall;
            InitialLayerPrintSpeedInfill = model.InitialLayerPrintSpeedInfill;
            InitialLayerPrintSpeedGeneral = model.InitialLayerPrintSpeedGeneral;
            InitialLayerTravelSpeed = model.InitialLayerTravelSpeed;
            WallSpeedEfficiency = model.WallSpeedEfficiency;
            InfillSpeedEfficiency = model.InfillSpeedEfficiency;
            WallCount = model.WallCount;
            InfillDensity = model.InfillDensity;
            SupportsEnabled = model.SupportsEnabled;
            SupportDensity = model.SupportDensity;
            SupportVolumeFactor = model.SupportVolumeFactor;
            PrintSpeedSupport = model.PrintSpeedSupport;
            SupportSpeedEfficiency = model.SupportSpeedEfficiency;
            SupportTravelFactor = model.SupportTravelFactor;
            TravelTimeFactor = model.TravelTimeFactor;
            CalibrationFactor = model.CalibrationFactor;

        }

        public void UpdateFromModel(PrinterProfile updated)
        {
            Model = updated ?? throw new ArgumentNullException(nameof(updated));
            Name = updated.Name;
            NozzleDiameter = updated.NozzleDiameter;
            LayerHeight = updated.LayerHeight;
            LineWidth = updated.LineWidth;
            InitialLayerHeight = updated.InitialLayerHeight;
            InitialLayerFlowGeneral = updated.InitialLayerFlowGeneral;
            InitialLayerFlowPerimeter = updated.InitialLayerFlowPerimeter;
            InitialLayerFlowInfill = updated.InitialLayerFlowInfill;
            FlowPercentGeneral = updated.FlowPercentGeneral;
            FlowPercentPerimeter = updated.FlowPercentPerimeter;
            FlowPercentInfill = updated.FlowPercentInfill;
            PrintSpeedGeneral = updated.PrintSpeedGeneral;
            PrintSpeedWall = updated.PrintSpeedWall;
            PrintSpeedInfill = updated.PrintSpeedInfill;
            TravelSpeed = updated.TravelSpeed;
            InitialLayerPrintSpeedWall = updated.InitialLayerPrintSpeedWall;
            InitialLayerPrintSpeedInfill = updated.InitialLayerPrintSpeedInfill;
            InitialLayerPrintSpeedGeneral = updated.InitialLayerPrintSpeedGeneral;
            InitialLayerTravelSpeed = updated.InitialLayerTravelSpeed;
            WallSpeedEfficiency = updated.WallSpeedEfficiency;
            InfillSpeedEfficiency = updated.InfillSpeedEfficiency;
            WallCount = updated.WallCount;
            InfillDensity = updated.InfillDensity;
            SupportsEnabled = updated.SupportsEnabled;
            SupportDensity = updated.SupportDensity;
            SupportVolumeFactor = updated.SupportVolumeFactor;
            PrintSpeedSupport = updated.PrintSpeedSupport;
            SupportSpeedEfficiency = updated.SupportSpeedEfficiency;
            SupportTravelFactor = updated.SupportTravelFactor;
            TravelTimeFactor = updated.TravelTimeFactor;
            CalibrationFactor = updated.CalibrationFactor;

            // Update other properties if needed for the UI
        }
    }
}
