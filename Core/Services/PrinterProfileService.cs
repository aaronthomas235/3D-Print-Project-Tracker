using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.Models;
using System;
using System.Collections.Generic;

namespace ThreeDPrintProjectTracker.Engine.Services
{
    public class PrinterProfileService : IPrinterProfileService
    {
        private readonly Dictionary<Guid, PrinterProfile> _profiles;
        public PrinterProfileService()
        {
            var defaultProfile = ReferencePrinterProfile.Default;
            _profiles = new Dictionary<Guid, PrinterProfile>()
            {
                [defaultProfile.Id] = defaultProfile,
            };
        }

        public PrinterProfile GetDefault()
        {
            return _profiles[Guid.Empty];
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
        }

        public void UpdateProfile(PrinterProfile profile)
        {
            if (!_profiles.ContainsKey(profile.Id))
            {
                throw new KeyNotFoundException($"Profile with ID {profile.Id} not found.");
            }
        }

        public bool RemoveProfile(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new InvalidOperationException("Cannot remove default/reference profile.");
            }

            return _profiles.Remove(id);
        }

        public IReadOnlyCollection<PrinterProfile> GetAllPrinterProfiles()
        {
            return _profiles.Values;
        }
    }
}
