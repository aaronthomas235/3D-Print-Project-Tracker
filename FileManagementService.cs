using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _3DPrintProjectTracker
{
    public class FileManagementService : InterfaceFileManagementService
    {
        public FileManagementService() {

        }

        public string[] GetProjectDirectories(string projectDirectoriesFilePath)
        {
            if (string.IsNullOrWhiteSpace(projectDirectoriesFilePath) || !Directory.Exists(projectDirectoriesFilePath))
            {
                return Array.Empty<string>();
            }
            return Directory.GetDirectories(projectDirectoriesFilePath, "*", SearchOption.TopDirectoryOnly);
        }
    }
}
