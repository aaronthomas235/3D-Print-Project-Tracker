using Core.Models;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IProjectTreeBuilderService
    {
        IReadOnlyList<ProjectTreeItem> BuildTree(string rootFolderPath);
    }
}
