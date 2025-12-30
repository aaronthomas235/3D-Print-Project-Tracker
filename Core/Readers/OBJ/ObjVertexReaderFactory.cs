using Core.Interfaces;

namespace Core.Readers.OBJ
{
    public static class ObjVertexReaderFactory
    {
        public static IVertexReader Create(string filePath)
        {
            return new ObjVertexReader(filePath);
        }
    }
}
