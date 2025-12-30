using Core.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Core.Readers.AMF
{
    public class AmfVertexReader : IVertexReader
    {
        private readonly string _filePath;
        public AmfVertexReader(string filePath) {
            _filePath = filePath;
        }

        public IEnumerator<(float X, float Y, float Z)> GetEnumerator()
        {
            using var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { IgnoreWhitespace = true });

            float measurementScale = 1f;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "amf")
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
                    float x = 0, y = 0, z = 0;

                    using var subReader = xmlReader.ReadSubtree();
                    while (subReader.Read())
                    {
                        if (subReader.NodeType == XmlNodeType.Element && subReader.Name == "x")
                        {
                            x = float.Parse(subReader.ReadElementContentAsString(), CultureInfo.InvariantCulture) * measurementScale;
                        }
                        if (subReader.NodeType == XmlNodeType.Element && subReader.Name == "y")
                        {
                            y = float.Parse(subReader.ReadElementContentAsString(), CultureInfo.InvariantCulture) * measurementScale;
                        }
                        if (subReader.NodeType == XmlNodeType.Element && subReader.Name == "z")
                        {
                            z = float.Parse(subReader.ReadElementContentAsString(), CultureInfo.InvariantCulture) * measurementScale;
                        }
                    }

                    yield return (x, y, z);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
