using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Services
{
    public class FileManagementService : IFileManagementService
    {
        private const string SaveFileName = "projectSaveData.json";

        public async Task<IReadOnlyList<ProjectTreeItem>> LoadProjectModelsAsync(string rootFolderPath)
        {
            if (string.IsNullOrWhiteSpace(rootFolderPath) || !Directory.Exists(rootFolderPath))
            {
                return Array.Empty<ProjectTreeItem>();
            }

            string saveFilePath = Path.Combine(rootFolderPath, SaveFileName);

            if (!File.Exists(saveFilePath))
            {
                return Array.Empty<ProjectTreeItem>();
            }

            string json = await File.ReadAllTextAsync(saveFilePath);

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            return JsonSerializer.Deserialize<List<ProjectTreeItem>>(json, options) ?? new List<ProjectTreeItem>();
        }

        public async Task SaveProjectsAsync(string rootFolderPath, IReadOnlyList<ProjectTreeItem> items)
        {
            if (string.IsNullOrWhiteSpace(rootFolderPath) || items == null)
            {
                return;
            }

            Directory.CreateDirectory(rootFolderPath);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string json = JsonSerializer.Serialize(items, options);
            string savePath = Path.Combine(rootFolderPath, SaveFileName);

            await File.WriteAllTextAsync(savePath, json);
        }
    }
}