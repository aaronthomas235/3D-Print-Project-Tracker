using Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Core.Readers.OBJ
{
    public class ObjVertexReader : IVertexReader
    {
        private readonly string _filePath;

        public ObjVertexReader(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerator<(float X, float Y, float Z)> GetEnumerator()
        {
            using var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            using var streamReader = new StreamReader(fileStream);

            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (!line.StartsWith("v "))
                {
                    continue;
                }

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4)
                {
                    continue;
                }

                float x = float.Parse(parts[1], CultureInfo.InvariantCulture) * 1000f;
                float y = float.Parse(parts[2], CultureInfo.InvariantCulture) * 1000f;
                float z = float.Parse(parts[3], CultureInfo.InvariantCulture) * 1000f;

                yield return (x, y, z);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
