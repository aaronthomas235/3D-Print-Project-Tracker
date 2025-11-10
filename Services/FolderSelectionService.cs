using _3DPrintProjectTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3DPrintProjectTracker.Services
{
    public class FolderSelectionService : IFolderSelectionService
    {
        public string SelectFolder(string folderBrowserDescription)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = folderBrowserDescription,
                UseDescriptionForTitle = true
            };
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }
}
