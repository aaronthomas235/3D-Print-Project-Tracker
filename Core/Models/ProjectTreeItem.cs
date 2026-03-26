using System;
using System.Collections.Generic;

namespace ThreeDPrintProjectTracker.Engine.Models
{
    public class ProjectTreeItem
    {
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public bool IsFile { get; set; }
        public Guid AssignedPrinterProfileId { get; set; }
        public List<ProjectTreeItem> Children { get; set; } = new();
    }
}
