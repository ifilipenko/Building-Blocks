using BuildingBlocks.Persistence.Configuration;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Persistence.SQLServer
{
    public class SqlServerConfigurationParameters : PersistenceConfigurationParameters<SqlServerConfigurationParameters>
    {
        public void SetRawConnectionString(string connectionString)
        {
            Condition.Requires(connectionString, "connectionString").IsNotNullOrEmpty();
            ConnectionString = connectionString;
        }
        
        public string ConnectionString { get; set; }

        public override SqlServerConfigurationParameters Clone()
        {
            var clone = base.Clone();
            clone.ConnectionString = ConnectionString;
            return clone;
        }
    }
}