using Core.Interfaces;

namespace Core.Readers._3MF
{
    public class MfVertexReaderFactory
    {
        public static IVertexReader Create(string filePath)
        {
            return new MfVertexReader(filePath);
        }
    }
}
