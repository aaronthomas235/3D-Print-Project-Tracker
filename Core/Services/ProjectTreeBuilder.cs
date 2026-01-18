using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class ProjectTreeBuilder : IProjectTreeBuilder
    {
        private readonly ISupportedFileFormatsService _supportedFileFormatsService;
        public ProjectTreeBuilder(ISupportedFileFormatsService supportedFileFormatsService)
        {
            _supportedFileFormatsService = supportedFileFormatsService;
        }

        public IReadOnlyList<ProjectTreeItem> BuildTree(string rootFolderPath)
        {
            if (string.IsNullOrWhiteSpace(rootFolderPath) || !Directory.Exists(rootFolderPath))
            {
                return Array.Empty<ProjectTreeItem>();
            }

            return BuildRecursive(rootFolderPath);
        }

        private List<ProjectTreeItem> BuildRecursive(string rootFolderPath)
        {
            var items = new List<ProjectTreeItem>();

            foreach(var directory in Directory.GetDirectories(rootFolderPath))
            {
                var node = new ProjectTreeItem
                {
                    Title = Path.GetFileName(directory),
                    Description = directory,
                    IsFile = false,
                    Children = BuildRecursive(directory)
                };

                items.Add(node);
            }

            foreach(var file in Directory.GetFiles(rootFolderPath))
            {
                if (!_supportedFileFormatsService.IsExtensionSupported(Path.GetExtension(file)))
                {
                    continue;
                }

                items.Add(new ProjectTreeItem
                {
                    Title = Path.GetFileName(file),
                    Description = file,
                    IsFile = true
                });
            }

            return items;
        }
    }
}
