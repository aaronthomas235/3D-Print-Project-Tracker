using ThreeDPrintProjectTracker.Engine.Models;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IPrintModelImportService
    {
        PrintModel ImportModel(string filePath);
    }
}
