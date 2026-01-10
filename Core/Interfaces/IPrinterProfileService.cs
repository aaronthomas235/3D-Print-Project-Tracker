using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
