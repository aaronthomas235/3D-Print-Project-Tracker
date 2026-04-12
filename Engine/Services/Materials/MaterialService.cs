using System;
using System.Collections.Generic;
using System.Linq;
using ThreeDPrintProjectTracker.Engine.Interfaces.Materials;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Engine.Services.Materials
{
    public class MaterialService : IMaterialService
    {
        private readonly Dictionary<Guid, MaterialDefinition> _materials;

        public event Action? MaterialsChanged;

        private readonly Guid _defaultMaterialId;

        public MaterialService()
        {
            var defaults = new[]
            {
                DefaultMaterialCatalog.Pla,
                DefaultMaterialCatalog.Abs,
                DefaultMaterialCatalog.Tpu
            };

            _materials = defaults.ToDictionary(m => m.Id, m => m);

            _defaultMaterialId = defaults[0].Id;
        }

        public MaterialDefinition GetDefault()
        {
            return _materials[_defaultMaterialId];
        }

        public MaterialDefinition? GetMaterialById(Guid id)
        {
            _materials.TryGetValue(id, out var material);
            return material;
        }

        public void AddMaterial(MaterialDefinition material)
        {
            if (_materials.ContainsKey(material.Id))
            {
                throw new InvalidOperationException($"Material with ID {material.Id} already exists.");
            }

            _materials[material.Id] = material;
            MaterialsChanged?.Invoke();
        }

        public void UpdateMaterial(MaterialDefinition material)
        {
            if (!_materials.ContainsKey(material.Id))
            {
                throw new KeyNotFoundException($"Material with ID {material.Id} not found.");
            }

            _materials[material.Id] = material;
            MaterialsChanged?.Invoke();
        }

        public bool RemoveMaterial(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot remove default/reference material.");
            }

            var removed = _materials.Remove(id);

            if (removed)
                MaterialsChanged?.Invoke();

            return removed;
        }

        public IReadOnlyCollection<MaterialDefinition> GetAllMaterials() => _materials.Values;
    }
}
