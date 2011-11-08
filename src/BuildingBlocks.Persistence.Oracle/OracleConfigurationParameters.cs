using BuildingBlocks.Persistence.Configuration;

namespace BuildingBlocks.Persistence.Oracle
{
    public class OracleConfigurationParameters : PersistenceConfigurationParameters<OracleConfigurationParameters>
    {
        public string ConnectionString { get; set; }
        public bool AllowUseOdpDriver { get; set; }

        public override OracleConfigurationParameters Clone()
        {
            var clone = base.Clone();
            clone.ConnectionString = ConnectionString;
            clone.AllowUseOdpDriver = AllowUseOdpDriver;
            return clone;
        }
    }
}