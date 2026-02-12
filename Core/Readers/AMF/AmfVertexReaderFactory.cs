using ThreeDPrintProjectTracker.Engine.Interfaces;

namespace ThreeDPrintProjectTracker.Engine.Readers.AMF
{
    public class AmfVertexReaderFactory
    {
        public static IVertexReader Create(string filePath)
        {
            return new AmfVertexReader(filePath);
        }
    }
}
