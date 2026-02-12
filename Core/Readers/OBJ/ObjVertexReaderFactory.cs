using ThreeDPrintProjectTracker.Engine.Interfaces;

namespace ThreeDPrintProjectTracker.Engine.Readers.OBJ
{
    public static class ObjVertexReaderFactory
    {
        public static IVertexReader Create(string filePath)
        {
            return new ObjVertexReader(filePath);
        }
    }
}
