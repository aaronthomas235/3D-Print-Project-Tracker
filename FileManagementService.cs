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

        public string[] GetProjectDirectories(string ProjectDirectoriesFilePath)
        {
            if (string.IsNullOrWhiteSpace(ProjectDirectoriesFilePath) || !Directory.Exists(ProjectDirectoriesFilePath))
            {
                return Array.Empty<string>();
            }
            return Directory.GetDirectories(ProjectDirectoriesFilePath, "*", SearchOption.TopDirectoryOnly);
        }

        public string[] GetProjectFiles(string ProjectFilesFilePath)
        {
            if (string.IsNullOrWhiteSpace(ProjectFilesFilePath) || !Directory.Exists(ProjectFilesFilePath))
            {
                return Array.Empty<string>();
            }
            return Directory.GetFiles(ProjectFilesFilePath, "*.stl");
        }
    }
}
