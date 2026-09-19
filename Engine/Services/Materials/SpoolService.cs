using System;
using System.Collections.Generic;
using ThreeDPrintProjectTracker.Engine.Interfaces.Materials;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Engine.Services.Materials
{
    public class SpoolService : ISpoolService
    {
        private readonly Dictionary<Guid, Spool> _spools;

        public event Action? SpoolsChanged;

        private readonly Guid _defaultSpoolId;

        public SpoolService()
        {
            var defaultSpool = DefaultSpools.Default;
            _defaultSpoolId = defaultSpool.Id;

            _spools = new Dictionary<Guid, Spool>()
            {
                [defaultSpool.Id] = defaultSpool,
            };
        }

        public Spool GetDefault()
        {
            return _spools[_defaultSpoolId];
        }

        public Spool? GetSpoolById(Guid id)
        {
            _spools.TryGetValue(id, out var spool);
            return spool;
        }

        public void AddSpool(Spool spool)
        {
            if (_spools.ContainsKey(spool.Id))
            {
                throw new InvalidOperationException($"Spool with ID {spool.Id} already exists.");
            }

            if (spool.Material == null)
            {
                throw new InvalidOperationException("Spool must have a material.");
            }

            _spools[spool.Id] = spool;
            SpoolsChanged?.Invoke();
        }

        public void UpdateSpool(Spool spool)
        {
            if (!_spools.ContainsKey(spool.Id))
            {
                throw new KeyNotFoundException($"Spool with ID {spool.Id} not found.");
            }

            if (spool.Material == null)
            {
                throw new InvalidOperationException("Spool must have a material.");
            }

            _spools[spool.Id] = spool;
            SpoolsChanged?.Invoke();
        }

        public bool RemoveSpool(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot remove default/reference spool.");
            }

            var removed = _spools.Remove(id);

            if (removed)
                SpoolsChanged?.Invoke();

            return removed;
        }

        public IReadOnlyCollection<Spool> GetAllSpools() => _spools.Values;
    }
}