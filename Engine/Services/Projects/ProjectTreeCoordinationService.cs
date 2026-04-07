using ThreeDPrintProjectTracker.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Interfaces.Projects;
using ThreeDPrintProjectTracker.Engine.Models.Projects;

namespace ThreeDPrintProjectTracker.Engine.Services
{
    public class ProjectTreeCoordinationService : IProjectTreeCoordinationService
    {
        private readonly IFileManagementService _fileManagementService;
        private readonly IProjectTreeBuilderService _projectTreeBuilderService;

        public ProjectTreeCoordinationService(IFileManagementService fileManagementService, IProjectTreeBuilderService projectTreeBuilderService)
        {
            _fileManagementService = fileManagementService ?? throw new ArgumentNullException(nameof(fileManagementService));
            _projectTreeBuilderService = projectTreeBuilderService ?? throw new ArgumentNullException(nameof(projectTreeBuilderService));
        }

        public async Task<IReadOnlyList<ProjectTreeItem>> LoadProjectsAsync(string rootFolder)
        {
            var models = await _fileManagementService.LoadProjectModelsAsync(rootFolder);

            if (models.Count == 0)
            {
                models = _projectTreeBuilderService.BuildTree(rootFolder);
            }

            return models;
        }

        public async Task SaveProjectsAsync(string rootFolder, IReadOnlyList<ProjectTreeItem> items)
        {
            await _fileManagementService.SaveProjectsAsync(rootFolder, items);
        }
    }
}
