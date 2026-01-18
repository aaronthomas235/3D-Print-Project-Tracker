using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProjectTreeItemViewModelFactory
    {
        ProjectTreeItemViewModel Create(ProjectTreeItem model);
    }
}
