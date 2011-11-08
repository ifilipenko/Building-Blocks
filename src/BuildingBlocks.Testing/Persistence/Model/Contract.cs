using System.Collections.Generic;

namespace BuildingBlocks.Testing.Persistence.Model
{
    public class Contract
    {
        public virtual long ContractID { get; set; }
        public virtual IList<TariffPlan> TariffPlans { get; set; }
    }
}