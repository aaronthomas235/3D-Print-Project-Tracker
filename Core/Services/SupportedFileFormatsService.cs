using Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Core.Services
{
    public class SupportedFileFormatsService : ISupportedFileFormatsService
    {
        private readonly HashSet<string> _supportedFileExtensionsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".stl", ".obj", ".3mf", ".dae", ".ply",
            ".gltf", ".glb", ".x3d", ".amf"
        };

        public bool IsExtensionSupported(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return false;
            }
            return _supportedFileExtensionsSet.Contains(extension);
        }
        public IReadOnlyCollection<string> GetSupportedFileExtensions() => _supportedFileExtensionsSet;
    }
}