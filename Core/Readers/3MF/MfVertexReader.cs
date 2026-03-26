using ThreeDPrintProjectTracker.Engine.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace ThreeDPrintProjectTracker.Engine.Readers._3MF
{
    public class MfVertexReader : IVertexReader
    {
        private readonly string _filePath;

        public MfVertexReader(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerator<(float X, float Y, float Z)> GetEnumerator()
        {
            using var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { IgnoreWhitespace = true });

            float measurementScale = 1f;

            while (xmlReader.Read()) {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "unit")
                {
                    var measurementUnit = xmlReader.GetAttribute("unit") ?? "millimeter";
                    measurementScale = measurementUnit.ToLower() switch
                    {
                        "millimeter" => 1f,
                        "centimeter" => 10f,
                        "meter" => 1000f,
                        "inch" => 25.4f,
                        _ => 1f
                    };
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "vertex")
                {
                    float x = float.Parse(xmlReader.GetAttribute("x")!, CultureInfo.InvariantCulture) * measurementScale;
                    float y = float.Parse(xmlReader.GetAttribute("y")!, CultureInfo.InvariantCulture) * measurementScale;
                    float z = float.Parse(xmlReader.GetAttribute("z")!, CultureInfo.InvariantCulture) * measurementScale;

                    yield return (x, y, z);
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
