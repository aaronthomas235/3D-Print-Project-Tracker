using System.Collections.Generic;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface ISupportedFileFormatsService
    {
        IReadOnlyCollection<string> GetSupportedFileExtensions();
        bool IsExtensionSupported(string extension);
    }
}