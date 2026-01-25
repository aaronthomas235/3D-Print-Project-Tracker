using Core.Models;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IProjectTreeBuilder
    {
        IReadOnlyList<ProjectTreeItem> BuildTree(string rootFolderPath);
    }
}
