using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Text;

namespace _3DPrintProjectTracker
{
    public class FileManagementService : InterfaceFileManagementService
    {
        private readonly HashSet<string> _supportedFileExtensionsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".stl", ".obj", ".3mf", ".dae", ".ply", ".gltf", ".glb", ".x3d", ".amf"
        };
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
            string[] projectFiles;
            if (string.IsNullOrWhiteSpace(ProjectFilesFilePath) || !Directory.Exists(ProjectFilesFilePath))
            {
                return Array.Empty<string>();
            }

            projectFiles = Directory.GetFiles(ProjectFilesFilePath)
                .Where(file => _supportedFileExtensionsSet.Contains(Path.GetExtension(file)))
                .ToArray();

            return projectFiles;
        }
    }
}
