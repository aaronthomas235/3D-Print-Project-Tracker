using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class PrinterProfileEditorViewModel : ObservableObject
    {
        // Backing model
        [ObservableProperty]
        private PrinterProfile _model;

        // UI helpers
        public IReadOnlyList<double> NozzleDiameterOptions { get; } = new[]
        {
        0.10, 0.20, 0.30, 0.40, 0.50, 0.60, 0.80, 1.00
    };

        // ───── General ─────
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private double nozzleDiameter;
        [ObservableProperty] private double layerHeight;
        [ObservableProperty] private double lineWidth;

        // ───── Initial Layer ─────
        [ObservableProperty] private double initialLayerHeight;
        [ObservableProperty] private double initialLayerLineWidth;
        [ObservableProperty] private double initialLayerFlowGeneral;
        [ObservableProperty] private double initialLayerFlowPerimeter;
        [ObservableProperty] private double initialLayerFlowInfill;

        // ───── Flow Multipliers ─────
        [ObservableProperty] private double flowPercentGeneral;
        [ObservableProperty] private double flowPercentPerimeter;
        [ObservableProperty] private double flowPercentInfill;

        // ───── Speeds ─────
        [ObservableProperty] private double printSpeedGeneral;
        [ObservableProperty] private double printSpeedWall;
        [ObservableProperty] private double printSpeedInfill;
        [ObservableProperty] private double travelSpeed;
        [ObservableProperty] private double initialLayerPrintSpeedWall;
        [ObservableProperty] private double initialLayerPrintSpeedInfill;
        [ObservableProperty] private double initialLayerPrintSpeedGeneral;
        [ObservableProperty] private double initialLayerTravelSpeed;

        // ───── Efficiency / Calibration ─────
        [ObservableProperty] private double wallSpeedEfficiency;
        [ObservableProperty] private double infillSpeedEfficiency;
        [ObservableProperty] private double travelTimeFactor;
        [ObservableProperty] private double calibrationFactor;

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

        public PrinterProfileEditorViewModel(PrinterProfile model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            LoadFromModel(model);

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName != nameof(HasChanges))
                    OnPropertyChanged(nameof(HasChanges));
            };
        }

        private void LoadFromModel(PrinterProfile model)
        {
            // General
            Name = model.Name;
            NozzleDiameter = model.NozzleDiameter;
            LayerHeight = model.LayerHeight;
            LineWidth = model.LineWidth;

            // Initial layer
            InitialLayerHeight = model.InitialLayerHeight;
            InitialLayerLineWidth = model.InitialLayerLineWidth;
            InitialLayerFlowGeneral = model.InitialLayerFlowGeneral;
            InitialLayerFlowPerimeter = model.InitialLayerFlowPerimeter;
            InitialLayerFlowInfill = model.InitialLayerFlowInfill;

            // Flow multipliers
            FlowPercentGeneral = model.FlowPercentGeneral;
            FlowPercentPerimeter = model.FlowPercentPerimeter;
            FlowPercentInfill = model.FlowPercentInfill;

            // Speeds
            PrintSpeedGeneral = model.PrintSpeedGeneral;
            PrintSpeedWall = model.PrintSpeedWall;
            PrintSpeedInfill = model.PrintSpeedInfill;
            TravelSpeed = model.TravelSpeed;
            InitialLayerPrintSpeedWall = model.InitialLayerPrintSpeedWall;
            InitialLayerPrintSpeedInfill = model.InitialLayerPrintSpeedInfill;
            InitialLayerPrintSpeedGeneral = model.InitialLayerPrintSpeedGeneral;
            InitialLayerTravelSpeed = model.InitialLayerTravelSpeed;

            // Efficiency / calibration
            WallSpeedEfficiency = model.WallSpeedEfficiency;
            InfillSpeedEfficiency = model.InfillSpeedEfficiency;
            TravelTimeFactor = model.TravelTimeFactor;
            CalibrationFactor = model.CalibrationFactor;

            // Geometry
            WallCount = model.WallCount;
            InfillDensity = model.InfillDensity;

            // Supports
            SupportsEnabled = model.SupportsEnabled;
            SupportDensity = model.SupportDensity;
            SupportVolumeFactor = model.SupportVolumeFactor;
            PrintSpeedSupport = model.PrintSpeedSupport;
            SupportSpeedEfficiency = model.SupportSpeedEfficiency;
            SupportTravelFactor = model.SupportTravelFactor;
        }

        /// <summary>
        /// True if any editor property differs from the underlying model
        /// </summary>
        public bool HasChanges =>
            Name != Model.Name ||
            NozzleDiameter != Model.NozzleDiameter ||
            LayerHeight != Model.LayerHeight ||
            LineWidth != Model.LineWidth ||
            InitialLayerHeight != Model.InitialLayerHeight ||
            InitialLayerLineWidth != Model.InitialLayerLineWidth ||
            InitialLayerFlowGeneral != Model.InitialLayerFlowGeneral ||
            InitialLayerFlowPerimeter != Model.InitialLayerFlowPerimeter ||
            InitialLayerFlowInfill != Model.InitialLayerFlowInfill ||
            FlowPercentGeneral != Model.FlowPercentGeneral ||
            FlowPercentPerimeter != Model.FlowPercentPerimeter ||
            FlowPercentInfill != Model.FlowPercentInfill ||
            PrintSpeedGeneral != Model.PrintSpeedGeneral ||
            PrintSpeedWall != Model.PrintSpeedWall ||
            PrintSpeedInfill != Model.PrintSpeedInfill ||
            TravelSpeed != Model.TravelSpeed ||
            InitialLayerPrintSpeedWall != Model.InitialLayerPrintSpeedWall ||
            InitialLayerPrintSpeedInfill != Model.InitialLayerPrintSpeedInfill ||
            InitialLayerPrintSpeedGeneral != Model.InitialLayerPrintSpeedGeneral ||
            InitialLayerTravelSpeed != Model.InitialLayerTravelSpeed ||
            WallSpeedEfficiency != Model.WallSpeedEfficiency ||
            InfillSpeedEfficiency != Model.InfillSpeedEfficiency ||
            TravelTimeFactor != Model.TravelTimeFactor ||
            CalibrationFactor != Model.CalibrationFactor ||
            WallCount != Model.WallCount ||
            InfillDensity != Model.InfillDensity ||
            SupportsEnabled != Model.SupportsEnabled ||
            SupportDensity != Model.SupportDensity ||
            SupportVolumeFactor != Model.SupportVolumeFactor ||
            PrintSpeedSupport != Model.PrintSpeedSupport ||
            SupportSpeedEfficiency != Model.SupportSpeedEfficiency ||
            SupportTravelFactor != Model.SupportTravelFactor;

        public void ApplyChanges()
        {
            Model = Model with
            {
                Name = Name,
                NozzleDiameter = NozzleDiameter,
                LayerHeight = LayerHeight,
                LineWidth = LineWidth,
                InitialLayerHeight = InitialLayerHeight,
                InitialLayerLineWidth = InitialLayerLineWidth,
                InitialLayerFlowGeneral = InitialLayerFlowGeneral,
                InitialLayerFlowPerimeter = InitialLayerFlowPerimeter,
                InitialLayerFlowInfill = InitialLayerFlowInfill,
                FlowPercentGeneral = FlowPercentGeneral,
                FlowPercentPerimeter = FlowPercentPerimeter,
                FlowPercentInfill = FlowPercentInfill,
                PrintSpeedGeneral = PrintSpeedGeneral,
                PrintSpeedWall = PrintSpeedWall,
                PrintSpeedInfill = PrintSpeedInfill,
                TravelSpeed = TravelSpeed,
                InitialLayerPrintSpeedWall = InitialLayerPrintSpeedWall,
                InitialLayerPrintSpeedInfill = InitialLayerPrintSpeedInfill,
                InitialLayerPrintSpeedGeneral = InitialLayerPrintSpeedGeneral,
                InitialLayerTravelSpeed = InitialLayerTravelSpeed,
                WallSpeedEfficiency = WallSpeedEfficiency,
                InfillSpeedEfficiency = InfillSpeedEfficiency,
                TravelTimeFactor = TravelTimeFactor,
                CalibrationFactor = CalibrationFactor,
                WallCount = WallCount,
                InfillDensity = InfillDensity,
                SupportsEnabled = SupportsEnabled,
                SupportDensity = SupportDensity,
                SupportVolumeFactor = SupportVolumeFactor,
                PrintSpeedSupport = PrintSpeedSupport,
                SupportSpeedEfficiency = SupportSpeedEfficiency,
                SupportTravelFactor = SupportTravelFactor
            };
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName != nameof(HasChanges))
            {
                base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasChanges)));
            }
        }
    }

}
