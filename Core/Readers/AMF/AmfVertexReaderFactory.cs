using Core.Interfaces;

namespace Core.Readers.AMF
{
    public class AmfVertexReaderFactory
    {
        public static IVertexReader Create(string filePath)
        {
            return new AmfVertexReader(filePath);
        }
    }
}
