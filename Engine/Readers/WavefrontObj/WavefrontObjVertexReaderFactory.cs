using ThreeDPrintProjectTracker.Engine.Interfaces;

namespace ThreeDPrintProjectTracker.Engine.Readers.OBJ
{
    public static class WavefrontObjVertexReaderFactory
    {
        public static IVertexReader Create(string filePath)
        {
            return new WavefrontObjVertexReader(filePath);
        }
    }
}
