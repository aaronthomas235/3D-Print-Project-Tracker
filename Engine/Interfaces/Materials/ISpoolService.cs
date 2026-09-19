using System;
using System.Collections.Generic;
using ThreeDPrintProjectTracker.Engine.Models.Materials;

namespace ThreeDPrintProjectTracker.Engine.Interfaces.Materials
{
    public interface ISpoolService
    {
        Spool GetDefault();
        Spool? GetSpoolById(Guid id);

        void AddSpool(Spool spool);
        void UpdateSpool(Spool spool);
        bool RemoveSpool(Guid id);
        IReadOnlyCollection<Spool> GetAllSpools();
    }
}
