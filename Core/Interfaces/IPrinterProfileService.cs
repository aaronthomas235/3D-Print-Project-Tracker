using Core.Models;
using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IPrinterProfileService
    {
        PrinterProfile GetDefault();
        PrinterProfile? GetPrinterProfileById(Guid id);

        void AddProfile(PrinterProfile profile);
        void UpdateProfile(PrinterProfile profile);
        bool RemoveProfile(Guid id);
        IReadOnlyCollection<PrinterProfile> GetAllPrinterProfiles();
    }
}
