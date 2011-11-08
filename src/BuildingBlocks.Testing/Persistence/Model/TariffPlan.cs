using System;
using System.Collections.Generic;

namespace BuildingBlocks.Testing.Persistence.Model
{
    public class TariffPlan
    {
        public TariffPlan()
        {
            Versions = new List<TariffPlanVersion>(0);
        }

        public virtual long TariffPlanID { get; set;}
        public virtual string Name { get; set; }
        public virtual IList<TariffPlanVersion> Versions { get; set; }

        public virtual void AddVersion(int versionNumber, DateTime? from, DateTime? to)
        {
            var version = new TariffPlanVersion {TariffPlan = this, Version = versionNumber};
            Versions.Add(version);
        }
    }
}