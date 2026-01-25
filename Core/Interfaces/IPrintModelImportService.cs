using Core.Models;

namespace Core.Interfaces
{
    public interface IPrintModelImportService
    {
        PrintModel ImportModel(string filePath);
    }
}
