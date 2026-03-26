using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.Models;
using ThreeDPrintProjectTracker.Engine.Readers.OBJ;
using ThreeDPrintProjectTracker.Engine.Readers.STL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ThreeDPrintProjectTracker.Engine.Services
{
    public class PrintModelImportService : IPrintModelImportService
    {
        private readonly Dictionary<string, Func<string, PrintModel>> _importers;
        public PrintModelImportService()
        {
            _importers = new(StringComparer.OrdinalIgnoreCase)
            {
                [".stl"] = StlImport,
                [".obj"] = ObjImport,
                [".3mf"] = MfImport,
                [".amf"] = AmfImport
            };
        }

        public PrintModel ImportModel(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            if (!_importers.TryGetValue(extension, out var importer))
            {
                throw new NotSupportedException($"Unsupported File Type: {extension}");
            }

            return importer(filePath);
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
            var vertices = reader.ToList();

            var triangles = TrianglesFromVertices(vertices);

            double volume = CalculateVolume(triangles);
            double surfaceArea = CalculateSurfaceArea(triangles);
            MeshDimensions dimensions = CalculateBoundingBox(vertices);

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
    }
}