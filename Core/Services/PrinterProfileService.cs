using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.Models;
using System;
using System.Collections.Generic;

namespace ThreeDPrintProjectTracker.Engine.Services
{
    public class PrinterProfileService : IPrinterProfileService
    {
        private readonly Dictionary<Guid, PrinterProfile> _profiles;

        public event Action? ProfilesChanged;

        private readonly Guid _defaultProfileId;
        public PrinterProfileService()
        {
            var defaultProfile = ReferencePrinterProfile.Default;
            _defaultProfileId = defaultProfile.Id;

            _profiles = new Dictionary<Guid, PrinterProfile>()
            {
                [defaultProfile.Id] = defaultProfile,
            };
        }

        public PrinterProfile GetDefault()
        {
            return _profiles[_defaultProfileId];
        }

        public PrinterProfile? GetPrinterProfileById(Guid id)
        {
            _profiles.TryGetValue(id, out var profile);
            return profile;
        }
        
        public void AddProfile(PrinterProfile profile)
        {
            if (_profiles.ContainsKey(profile.Id))
            {
                throw new InvalidOperationException($"Profile with ID {profile.Id} already exists.");
            }

            _profiles[profile.Id] = profile;
            ProfilesChanged?.Invoke();
        }

        public void UpdateProfile(PrinterProfile profile)
        {
            if (!_profiles.ContainsKey(profile.Id))
            {
                throw new KeyNotFoundException($"Profile with ID {profile.Id} not found.");
            }

            _profiles[profile.Id] = profile;
            ProfilesChanged?.Invoke();
        }

        public bool RemoveProfile(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot remove default/reference profile.");
            }

            var removed = _profiles.Remove(id);
            if (removed)
                ProfilesChanged?.Invoke();

            return removed;
        }

        public IEnumerable<PrinterProfile> GetAllPrinterProfiles() => _profiles.Values;
    }
}
