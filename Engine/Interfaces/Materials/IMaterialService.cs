using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Engine.Interfaces.Materials
{
    public interface IMaterialService
    {
        event Action? MaterialsChanged;
        MaterialDefinition GetDefault();
        MaterialDefinition? GetMaterialById(Guid id);

        void AddMaterial(MaterialDefinition material);
        void UpdateMaterial(MaterialDefinition material);
        bool RemoveMaterial(Guid id);

        IReadOnlyCollection<MaterialDefinition> GetAllMaterials();
    }
}
