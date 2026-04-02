using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IPrintModelImportService
    {
        PrintModel ImportModel(string filePath);
    }
}
