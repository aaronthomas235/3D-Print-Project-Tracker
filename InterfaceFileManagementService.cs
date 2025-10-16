using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DPrintProjectTracker
{
    public interface InterfaceFileManagementService
    {
        string[] GetProjectDirectories(string ProjectDirectoriesFilePath);
        string[] GetProjectFiles(string ProjectFilesFilePath);
    }
}
