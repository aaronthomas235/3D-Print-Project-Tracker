using Core.Interfaces;
using Core.Models;
using Core.Readers.OBJ;
using Core.Readers.STL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services
{
    public class MeshAnalyserService : IMeshAnalyserService
    {
        private readonly Dictionary<string, Func<string, MeshDimensions>> _analysers;

        public MeshAnalyserService()
        {
            _analysers = new(StringComparer.OrdinalIgnoreCase)
            {
                [".stl"] = StlAnalysis,
                [".obj"] = ObjAnalysis,
                [".3mf"] = MfAnalysis,
                [".amf"] = AmfAnalysis

            };
        }
        public async Task<MeshDimensions> AnalyseAsync(string filePath)
        {
            return await Task.Run(() => {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();

                if (!_analysers.TryGetValue(extension, out var analyser))
                {
                    throw new NotSupportedException($"Unsupported File Type: {extension}");
                }

                return analyser(filePath);
            });
        }

        private MeshDimensions StlAnalysis(string filePath)
        {
            IVertexReader vertexReader = StlVertexReaderFactory.Create(filePath);
            return CalculateBoundingBox(vertexReader);
        }

        private MeshDimensions ObjAnalysis(string filePath)
        {
            IVertexReader vertexReader = ObjVertexReaderFactory.Create(filePath);
            return CalculateBoundingBox(vertexReader);
        }

        private MeshDimensions MfAnalysis(string filePath)
        {
            IVertexReader vertexReader = StlVertexReaderFactory.Create(filePath);
            return CalculateBoundingBox(vertexReader);
        }

        private MeshDimensions AmfAnalysis(string filePath)
        {
            IVertexReader vertexReader = StlVertexReaderFactory.Create(filePath);
            return CalculateBoundingBox(vertexReader);
        }


        private MeshDimensions CalculateBoundingBox(IVertexReader vertices)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            foreach (var (x, y, z) in vertices)
            {
                if (x < minX) minX = x; if (x > maxX) maxX = x;
                if (y < minY) minY = y; if (y > maxY) maxY = y;
                if (z < minZ) minZ = z; if (z > maxZ) maxZ = z;
            }

            if (!vertices.Any())
            {
                return new MeshDimensions(1, 1, 1);
            }

            return new MeshDimensions(maxX - minX, maxY - minY, maxZ - minZ);
        }
    }
}
