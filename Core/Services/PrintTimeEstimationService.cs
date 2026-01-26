using Core.Interfaces;
using Core.Models;
using System;
using System.Threading.Tasks;

namespace Core.Services
{
    public class PrintTimeEstimationService : IPrintTimeEstimationService
    {
        private const double _minimumFlowMm3PerSecond = 0.001;

        public PrintTimeEstimationService()
        {

        }

        public async Task<TimeSpan> EstimatePrintTimeAsync(PrintModel model, PrinterProfile profile)
        {
            return EstimateTime(model, profile);
        }

        private static TimeSpan EstimateTime(PrintModel model, PrinterProfile profile)
        {
            if (model == null || profile == null)
            {
                throw new ArgumentNullException("Model and Profile cannot be null.");
            }
            if (model.VolumeMm3 <= 0 || model.SurfaceAreaMm2 <= 0 || model.HeightMm <= 0)
            {
                return TimeSpan.Zero;
            }

            LayerCalculation firstLayer = CalculateFirstLayerTime(model, profile);
            double mainLayers = CalculateMainLayersTime(model, profile, firstLayer);

            double extrusionTime = firstLayer.TimeSeconds + mainLayers;

            double travelTime = CalculateTravelTime(extrusionTime, profile);

            double supportsTime = CalculateSupportTime(model, profile);

            double calibration = Math.Clamp(profile.CalibrationFactor, 0.5, 2.5);

            double totalSeconds = (extrusionTime + travelTime + supportsTime) * calibration;


            return TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
        }

        private sealed record LayerCalculation(
            double HeightMm,
            double VolumeMm3,
            double PerimeterVolumeMm3,
            double InfillVolumeMm3,
            double TimeSeconds
        );

        private static LayerCalculation CalculateFirstLayerTime(PrintModel model, PrinterProfile profile)
        {
            double height = Math.Min(profile.InitialLayerHeight, model.HeightMm);
            double volume = model.VolumeMm3 * (height / model.HeightMm);

            double perimeterFlowMultiplier = ResolveFlowMultiplier(profile.FlowPercentPerimeter);
            double infillFlowMultiplier = ResolveFlowMultiplier(profile.FlowPercentInfill);

            double wallSpeed = ResolveEffectiveSpeed(profile.InitialLayerPrintSpeedWall, profile.InitialLayerPrintSpeedGeneral, profile.WallSpeedEfficiency);
            double infillSpeed = ResolveEffectiveSpeed(profile.InitialLayerPrintSpeedInfill, profile.InitialLayerPrintSpeedGeneral, profile.InfillSpeedEfficiency);

            double perimeterLength = model.SurfaceAreaMm2 / height;
            double perimeterVolume = perimeterLength * profile.InitialLayerLineWidth * height * profile.WallCount * perimeterFlowMultiplier;
            double perimeterFlow = profile.InitialLayerLineWidth * height * wallSpeed;

            double infillVolume = volume * profile.InfillDensity * infillFlowMultiplier;
            double infillFlow = profile.InitialLayerLineWidth * height * infillSpeed;

            double timeSeconds = CalculateTimeInSeconds(perimeterVolume, perimeterFlow) + CalculateTimeInSeconds(infillVolume, infillFlow);

            return new LayerCalculation(
                height,
                volume,
                perimeterVolume,
                infillVolume,
                timeSeconds);
        }

        private static double CalculateMainLayersTime(PrintModel model, PrinterProfile profile, LayerCalculation firstLayerCalculations)
        {
            double remainingVolume = Math.Max(0, model.VolumeMm3 - firstLayerCalculations.VolumeMm3);
            if (remainingVolume <= 0)
            {
                return 0;
            }

            double perimeterFlowMultiplier = ResolveFlowMultiplier(profile.FlowPercentPerimeter);
            double infillFlowMultiplier = ResolveFlowMultiplier(profile.FlowPercentInfill);

            double wallSpeed = ResolveEffectiveSpeed(profile.PrintSpeedWall, profile.PrintSpeedGeneral, profile.WallSpeedEfficiency);
            double infillSpeed = ResolveEffectiveSpeed(profile.PrintSpeedInfill, profile.PrintSpeedGeneral, profile.InfillSpeedEfficiency);

            double perimeterLength = model.SurfaceAreaMm2 / profile.LayerHeight;
            double perimeterVolume = perimeterLength * profile.LineWidth * profile.LayerHeight * profile.WallCount * perimeterFlowMultiplier;
            double perimeterFlow = profile.LineWidth * profile.LayerHeight * wallSpeed;

            double infillVolume = remainingVolume * profile.InfillDensity * infillFlowMultiplier;
            double infillFlow = profile.LineWidth * profile.LayerHeight * infillSpeed;

            double perimeterTime = CalculateTimeInSeconds(perimeterVolume, perimeterFlow);
            double infillTime = CalculateTimeInSeconds(infillVolume, infillFlow);

            return perimeterTime + infillTime;
        }

        private static double CalculateTravelTime(double extrusionTimeSeconds, PrinterProfile profile)
        {
            double factor = Math.Clamp(profile.TravelTimeFactor, 0.05, 0.5);

            return extrusionTimeSeconds * factor;
        }

        private static double CalculateSupportTime(PrintModel model, PrinterProfile profile)
        {
            if (!profile.SupportsEnabled)
            {
                return 0;
            }

            double supportVolume = model.VolumeMm3 * profile.SupportVolumeFactor * profile.SupportDensity;

            double supportSpeed = ResolveEffectiveSpeed(profile.PrintSpeedSupport, profile.PrintSpeedGeneral, profile.SupportSpeedEfficiency);

            double supportFlow = profile.LineWidth * profile.LayerHeight * supportSpeed;

            double printTime = CalculateTimeInSeconds(supportVolume, supportFlow);

            double travelOverhead = printTime * profile.SupportTravelFactor;

            return printTime + travelOverhead;
        }

        private static double ResolveEffectiveSpeed( double specificSpeed, double generalSpeed, double efficiency)
        {
            double baseSpeed = specificSpeed > 0
                ? specificSpeed
                : Math.Max(generalSpeed, 0);

            return baseSpeed * Math.Clamp(efficiency, 0.1, 1.0);
        }

        private static double ResolveFlowMultiplier(double flowPercent)
        {
            return Math.Max(flowPercent, 0.01) / 100.0;
        }

        private static double ClampFlow(double flowMm3PerSecond)
        {
            return Math.Max(flowMm3PerSecond, _minimumFlowMm3PerSecond);
        }

        private static double CalculateTimeInSeconds(double volumeMm3, double flowMm3PerSecond)
        {
            double safeFlow = ClampFlow(flowMm3PerSecond);
            return volumeMm3 / safeFlow;
        }
    }
}
