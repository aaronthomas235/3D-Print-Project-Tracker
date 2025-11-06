using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DPrintProjectTracker.Interfaces
{
    public interface IFolderSelectionService
    {
        string SelectFolder(string folderBrowserDescription);
    }
}
