using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface ISupportedFileFormatsService
    {
        IReadOnlyCollection<string> GetSupportedFileExtensions();
        bool IsExtensionSupported(string extension);
    }
}