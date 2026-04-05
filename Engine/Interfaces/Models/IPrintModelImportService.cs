using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Interfaces.Models
{
    public interface IPrintModelImportService
    {
        PrintModel ImportModel(string filePath);
    }
}
