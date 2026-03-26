using ThreeDPrintProjectTracker.Engine.Models;
using System.Collections.Generic;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IProjectTreeBuilderService
    {
        IReadOnlyList<ProjectTreeItem> BuildTree(string rootFolderPath);
    }
}
