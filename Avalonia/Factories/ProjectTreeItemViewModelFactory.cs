using System;
using System.Collections.ObjectModel;
using System.Linq;
using ThreeDPrintProjectTracker.Avalonia.Interfaces;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;
using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.Interfaces.Analysis;
using ThreeDPrintProjectTracker.Engine.Interfaces.Infrastructure;
using ThreeDPrintProjectTracker.Engine.Interfaces.Models;
using ThreeDPrintProjectTracker.Engine.Interfaces.Printing;
using ThreeDPrintProjectTracker.Engine.Models.Projects;

namespace ThreeDPrintProjectTracker.Avalonia.Factories
{
    public class ProjectTreeItemViewModelFactory : IProjectTreeItemViewModelFactory
    {
        private readonly IPrintItemAnalysisService _printItemAnalysisService;

        public ProjectTreeItemViewModelFactory(IPrintItemAnalysisService printItemAnalysisService)
        {
            _printItemAnalysisService = printItemAnalysisService;
        }

        public ProjectTreeItemViewModel Create(ProjectTreeItem model)
        {
            return Create(model, null);
        }

        private ProjectTreeItemViewModel Create(ProjectTreeItem model, ProjectTreeItemViewModel? parent)
        {
            var vm = new ProjectTreeItemViewModel(model, _printItemAnalysisService, parent);

            foreach (var childModel in model.Children)
            {
                var childVm = Create(childModel, vm);
                vm.Children.Add(childVm);
            }

            return vm;
        }
    }
}
