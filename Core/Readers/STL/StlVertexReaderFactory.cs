using Core.Interfaces;
using System;
using System.IO;

namespace Core.Readers.STL
{
    public static class StlVertexReaderFactory
    {
        public static IVertexReader Create(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var streamReader = new StreamReader(fileStream, leaveOpen: true);

            char[] buffer = new char[5];
            streamReader.Read(buffer, 0, 5);

            fileStream.Seek(0, SeekOrigin.Begin);

            if (new string(buffer).Equals("solid", StringComparison.OrdinalIgnoreCase))
            {
                return new StlAsciiVertexReader(filePath);
            }

            return new StlBinaryVertexReader(filePath);
        }
    }
}
