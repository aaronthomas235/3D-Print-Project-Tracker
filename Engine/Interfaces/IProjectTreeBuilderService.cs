using System.Collections.Generic;
using ThreeDPrintProjectTracker.Engine.Models.Projects;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IProjectTreeBuilderService
    {
        IReadOnlyList<ProjectTreeItem> BuildTree(string rootFolderPath);
    }
}
