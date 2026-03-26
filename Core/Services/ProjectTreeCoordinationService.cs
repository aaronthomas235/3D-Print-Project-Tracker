using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Services
{
    public class ProjectTreeCoordinationService : IProjectTreeCoordinationService
    {
        private readonly IFileManagementService _fileManagementService;
        private readonly IProjectTreeBuilderService _projectTreeBuilderService;
        private readonly IProjectTreeItemViewModelFactory _projectTreeItemViewModelFactory;

        public ProjectTreeCoordinationService(IFileManagementService fileManagementService, IProjectTreeBuilderService projectTreeBuilderService, IProjectTreeItemViewModelFactory projectTreeItemViewModelFactory)
        {
            _fileManagementService = fileManagementService ?? throw new ArgumentNullException(nameof(fileManagementService));
            _projectTreeBuilderService = projectTreeBuilderService ?? throw new ArgumentNullException(nameof(projectTreeBuilderService));
            _projectTreeItemViewModelFactory = projectTreeItemViewModelFactory ?? throw new ArgumentNullException(nameof(projectTreeItemViewModelFactory));
        }

        public async Task<IReadOnlyList<ProjectTreeItemViewModel>> LoadProjectsAsync(string rootFolder)
        {
            var models = await _fileManagementService.LoadProjectModelsAsync(rootFolder);

            if (models.Count == 0)
            {
                models = _projectTreeBuilderService.BuildTree(rootFolder);
            }

            var vms = models
                .Select(vm => _projectTreeItemViewModelFactory.Create(vm))
                .ToList();

            CollapseAll(vms);
            return vms;
        }

        public async Task SaveProjectsAsync(string rootFolder, IReadOnlyList<ProjectTreeItemViewModel> items)
        {
            var models = items.Select(vm => vm.ToModel()).ToList();

            await _fileManagementService.SaveProjectsAsync(rootFolder, models);
        }

        private void CollapseChildren(ProjectTreeItemViewModel parent)
        {
            foreach (var child in parent.Children)
            {
                child.IsExpanded = false;
                CollapseChildren(child);
            }
        }

        private void CollapseAll(IEnumerable<ProjectTreeItemViewModel> items)
        {
            foreach (var item in items)
            {
                item.IsExpanded = false;
                CollapseAll(item.Children);
            }
        }
    }
}
