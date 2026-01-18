using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class ProjectTreeItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsFile { get; set; }
        public Guid AssignedPrinterProfileId { get; set; }
        public List<ProjectTreeItem> Children { get; set; } = new();
    }
}
