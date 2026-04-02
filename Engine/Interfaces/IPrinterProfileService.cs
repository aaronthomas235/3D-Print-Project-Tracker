using System;
using System.Collections.Generic;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IPrinterProfileService
    {
        PrinterProfile GetDefault();
        PrinterProfile? GetPrinterProfileById(Guid id);

        void AddProfile(PrinterProfile profile);
        void UpdateProfile(PrinterProfile profile);
        bool RemoveProfile(Guid id);
        IEnumerable<PrinterProfile> GetAllPrinterProfiles();
    }
}
