using Core.Interfaces;
using Core.Models;
using System;
using System.Threading.Tasks;

namespace Core.Services
{
    public class MaterialUsageEstimationService :IMaterialUsageEstimationService
    {
        private readonly IPrintModelCacheService _modelCacheService;
        private const double _filamentDiameterMm = 1.75;
        private const double _filamentDensityGPerMm3 = 0.00124;
        private static readonly double _filamentAreaMm2 = Math.PI * Math.Pow(_filamentDiameterMm / 2.0, 2.0);

        public MaterialUsageEstimationService(IPrintModelCacheService modelCacheService)
        {
            _modelCacheService = modelCacheService;
        }

        public async Task<string> EstimateWeightAsync(string filePath, PrinterProfile profile)
        {

            var model = await _modelCacheService.GetPrintModelAsync(filePath);
            var estimate = EstimateMaterialUsage(model, profile);

            return FormatWeight(estimate.WeightGrams);
        }

        private static MaterialEstimate EstimateMaterialUsage(PrintModel model, PrinterProfile profile)
        {
            if (model.VolumeMm3 <= 0 || model.HeightMm <= 0)
            {
                return new MaterialEstimate(0, 0, 0);
            }

            LayerCalculation firstLayer = CalculateFirstLayer(model, profile);

            double mainVolume = CalculateMainLayersVolume(model, profile, firstLayer);

            double supportVolume = CalculateSupportVolume(model, profile);

            double totalVolume = firstLayer.PerimeterVolumeMm3 + firstLayer.InfillVolumeMm3 + mainVolume + supportVolume;

            double filamentLengthMm = totalVolume / _filamentAreaMm2;

            double filamentLengthM = filamentLengthMm / 1000.0;

            double weightGrams = totalVolume * _filamentDensityGPerMm3;

            return new MaterialEstimate(totalVolume, filamentLengthM, weightGrams);
        }

        private static string FormatWeight(double grams)
        {
            return grams < 1
                ? $"{grams * 1000:0}mg"
                : $"{grams:0}g";
        }

        private sealed record MaterialEstimate(
            double VolumeMm3,
            double FilamentLengthMeters,
            double WeightGrams
        );

        private sealed record LayerCalculation(
            double HeightMm,
            double VolumeMm3,
            double PerimeterVolumeMm3,
            double InfillVolumeMm3
        );

        private static LayerCalculation CalculateFirstLayer(PrintModel model, PrinterProfile profile)
        {
            double height = Math.Min(profile.InitialLayerHeight, model.HeightMm);
            double volume = model.VolumeMm3 * (height / model.HeightMm);

            double perimeterFlowMultiplier = ResolveExtrusionMultiplier(profile.FlowPercentPerimeter);
            double infillFlowMultiplier = ResolveExtrusionMultiplier(profile.FlowPercentInfill);

            double perimeterLength = model.SurfaceAreaMm2 / height;
            double perimeterVolume = perimeterLength * profile.InitialLayerLineWidth * height * profile.WallCount * perimeterFlowMultiplier;

            double infillVolume = volume * profile.InfillDensity * infillFlowMultiplier;

            return new LayerCalculation(
                height,
                volume,
                perimeterVolume,
                infillVolume);
        }

        private static double CalculateMainLayersVolume(PrintModel model, PrinterProfile profile, LayerCalculation firstLayerCalculations)
        {
            double remainingVolume = Math.Max(0, model.VolumeMm3 - firstLayerCalculations.VolumeMm3);
            if (remainingVolume <= 0)
            {
                return 0;
            }

            double perimeterFlowMultiplier = ResolveExtrusionMultiplier(profile.FlowPercentPerimeter);
            double infillFlowMultiplier = ResolveExtrusionMultiplier(profile.FlowPercentInfill);


            double perimeterLength = model.SurfaceAreaMm2 / profile.LayerHeight;
            double perimeterVolume = perimeterLength * profile.LineWidth * profile.LayerHeight * profile.WallCount * perimeterFlowMultiplier;

            double infillVolume = remainingVolume * profile.InfillDensity * infillFlowMultiplier;

            return perimeterVolume + infillVolume;
        }

        private static double CalculateSupportVolume(PrintModel model, PrinterProfile profile)
        {
            if (!profile.SupportsEnabled)
            {
                return 0;
            }

            double supportVolume = model.VolumeMm3 * profile.SupportVolumeFactor * profile.SupportDensity;

            return supportVolume;
        }

        private static double ResolveExtrusionMultiplier(double flowPercent)
        {
            return Math.Max(flowPercent, 0.01) / 100.0;
        }
    }
}
