using ThreeDPrintProjectTracker.Engine.Interfaces;

namespace ThreeDPrintProjectTracker.Engine.Readers._3MF
{
    public class MfVertexReaderFactory
    {
        public static IVertexReader Create(string filePath)
        {
            return new MfVertexReader(filePath);
        }
    }
}
