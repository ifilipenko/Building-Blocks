namespace BuildingBlocks.Testing.Persistence.Model
{
    public class TariffPlanVersion
    {
        public virtual long TariffPlanVersionID { get; set; }
        public virtual int Version { get; set; }
        public virtual TariffPlan TariffPlan { get; set; }
    }
}