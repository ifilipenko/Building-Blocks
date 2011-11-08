using BuildingBlocks.Persistence.Configuration;
using FluentNHibernate.Cfg.Db;
using NHibernate.Driver;

namespace BuildingBlocks.Persistence.Oracle
{
    public class OraclePersistenceConfigurationItem : PersistenceConfigurationItem<OracleConfigurationParameters>
    {
        public OraclePersistenceConfigurationItem(OracleConfigurationParameters configurationParameters) 
            : base(configurationParameters)
        {
        }

        protected override IPersistenceConfigurer SetupDatabaseConnection()
        {
            var persistanceConfiguration = OracleDataClientConfiguration.Oracle9;
            if (ConfigurationParameters.AllowUseOdpDriver)
            {
                persistanceConfiguration.Driver<StringClobFixOracleDataClientDriver>();
            }
            else
            {
                persistanceConfiguration.Driver<OracleClientDriver>();
            }
            persistanceConfiguration.ConnectionString(ConfigurationParameters.ConnectionString)
                .Dialect<NHibernate.Dialect.Oracle9iDialect>()
                .Provider<NHibernate.Connection.DriverConnectionProvider>();

            return persistanceConfiguration;
        }
    }
}