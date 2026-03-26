using ThreeDPrintProjectTracker.Engine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Readers.STL
{
    public class StlBinaryVertexReader : IVertexReader
    {
        private readonly string _filePath;

        public StlBinaryVertexReader(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerator<(float X, float Y, float Z)> GetEnumerator()
        {
            using var fileStreamer = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStreamer);

            binaryReader.ReadBytes(80);
            uint trianglesCount = binaryReader.ReadUInt32();
            for (uint index = 0; index < trianglesCount; index++)
            {
                binaryReader.ReadBytes(12);
                for (int vertexIndex = 0; vertexIndex < 3; vertexIndex++)
                {
                    float x = binaryReader.ReadSingle();
                    float y = binaryReader.ReadSingle();
                    float z = binaryReader.ReadSingle();
                    yield return (x, y, z);
                }
                binaryReader.ReadBytes(2);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
