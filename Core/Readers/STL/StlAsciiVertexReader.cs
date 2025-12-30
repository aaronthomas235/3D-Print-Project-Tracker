using Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Core.Readers.STL
{
    public class StlAsciiVertexReader : IVertexReader
    {
        private readonly string _filePath;

        public StlAsciiVertexReader(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerator<(float X, float Y, float Z)> GetEnumerator()
        {
            using var streamReader = new StreamReader(_filePath);
            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("vertex", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4 &&
                        float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                        float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                        float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
                    {
                        yield return (x, y, z);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
