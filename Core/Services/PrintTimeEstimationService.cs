using Core.Interfaces;
using Core.Models;
using Core.Readers.OBJ;
using Core.Readers.STL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services
{
    public class PrintTimeEstimationService : IPrintTimeEstimationService
    {
        private readonly Dictionary<string, Func<string, PrintModel>> _importers;
        private const double _minimumFlowMm3PerSecond = 0.001;

        public PrintTimeEstimationService()
        {
            _importers = new(StringComparer.OrdinalIgnoreCase)
            {
                [".stl"] = StlImport,
                [".obj"] = ObjImport,
                [".3mf"] = MfImport,
                [".amf"] = AmfImport
            };
        }

        public async Task<TimeSpan> EstimateAsync(string filePath, PrinterProfile profile)
        {
            return await Task.Run(() =>
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();

                if (!_importers.TryGetValue(extension, out var importer))
                    throw new NotSupportedException($"Unsupported File Type: {extension}");

                var model = importer(filePath);
                return EstimateTime(model, profile);
            });
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

        private static PrintModel StlImport(string filePath)
        {
            var vertexReader = StlVertexReaderFactory.Create(filePath);
            return BuildPrintModel(vertexReader);
        }

        private static PrintModel ObjImport(string filePath)
        {
            IVertexReader vertexReader = ObjVertexReaderFactory.Create(filePath);
            return BuildPrintModel(vertexReader);
        }

        private static PrintModel MfImport(string filePath)
        {
            IVertexReader vertexReader = StlVertexReaderFactory.Create(filePath);
            return BuildPrintModel(vertexReader);
        }

        private static PrintModel AmfImport(string filePath)
        {
            IVertexReader vertexReader = StlVertexReaderFactory.Create(filePath);
            return BuildPrintModel(vertexReader);
        }

        private static PrintModel BuildPrintModel(IVertexReader reader)
        {
            var triangles = TrianglesFromVertices(reader);

            double volume = CalculateVolume(triangles);
            double surfaceArea = CalculateSurfaceArea(triangles);
            MeshDimensions dimensions = CalculateBoundingBox(reader);

            return new PrintModel
            {
                VolumeMm3 = volume,
                SurfaceAreaMm2 = surfaceArea,
                Dimensions = dimensions,
                HeightMm = dimensions.Height
            };
        }

        private readonly struct Triangle
        {
            public readonly float X1, Y1, Z1;
            public readonly float X2, Y2, Z2;
            public readonly float X3, Y3, Z3;

            public Triangle(float x1, float y1, float z1,
                            float x2, float y2, float z2,
                            float x3, float y3, float z3)
            {
                X1 = x1; Y1 = y1; Z1 = z1;
                X2 = x2; Y2 = y2; Z2 = z2;
                X3 = x3; Y3 = y3; Z3 = z3;
            }
        }

        private static IEnumerable<Triangle> TrianglesFromVertices(IEnumerable<(float x, float y, float z)> vertices)
        {
            using var verticesEnumerator = vertices.GetEnumerator();
            while (true)
            {
                if (!verticesEnumerator.MoveNext()) yield break; var v1 = verticesEnumerator.Current;
                if (!verticesEnumerator.MoveNext()) yield break; var v2 = verticesEnumerator.Current;
                if (!verticesEnumerator.MoveNext()) yield break; var v3 = verticesEnumerator.Current;

                yield return new Triangle(v1.x, v1.y, v1.z, v2.x, v2.y, v2.z, v3.x, v3.y, v3.z);
            }
        }

        private static double CalculateVolume(IEnumerable<Triangle> triangles)
        {
            double volume = 0;
            foreach (var t in triangles)
            {
                var crossX = t.Y2 * t.Z3 - t.Z2 * t.Y3;
                var crossY = t.Z2 * t.X3 - t.X2 * t.Z3;
                var crossZ = t.X2 * t.Y3 - t.Y2 * t.X3;

                var dot = t.X1 * crossX + t.Y1 * crossY + t.Z1 * crossZ;
                volume += dot / 6.0;
            }
            return Math.Abs(volume);
        }

        private static double CalculateSurfaceArea(IEnumerable<Triangle> triangles)
        {
            double totalArea = 0;
            foreach (var t in triangles)
            {
                var abX = t.X2 - t.X1;
                var abY = t.Y2 - t.Y1;
                var abZ = t.Z2 - t.Z1;

                var acX = t.X3 - t.X1;
                var acY = t.Y3 - t.Y1;
                var acZ = t.Z3 - t.Z1;

                var crossX = abY * acZ - abZ * acY;
                var crossY = abZ * acX - abX * acZ;
                var crossZ = abX * acY - abY * acX;

                totalArea += 0.5 * Math.Sqrt(crossX * crossX + crossY * crossY + crossZ * crossZ);
            }
            return totalArea;
        }

        private static MeshDimensions CalculateBoundingBox(IEnumerable<(float x, float y, float z)> reader)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            foreach (var (x, y, z) in reader)
            {
                minX = Math.Min(minX, x); maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y); maxY = Math.Max(maxY, y);
                minZ = Math.Min(minZ, z); maxZ = Math.Max(maxZ, z);
            }

            if (!reader.Any())
                return new MeshDimensions(1, 1, 1);

            return new MeshDimensions(maxX - minX, maxY - minY, maxZ - minZ);
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
            return Math.Max(flowMm3PerSecond, _minimumFlowMm3PerSecond); // 0.001
        }

        private static double CalculateTimeInSeconds(double volumeMm3, double flowMm3PerSecond)
        {
            double safeFlow = ClampFlow(flowMm3PerSecond);
            return volumeMm3 / safeFlow;
        }
    }
}
