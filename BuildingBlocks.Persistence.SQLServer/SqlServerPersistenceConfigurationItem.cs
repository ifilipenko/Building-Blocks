using BuildingBlocks.Persistence.Configuration;
using FluentNHibernate.Cfg.Db;

namespace BuildingBlocks.Persistence.SQLServer
{
    public class SqlServerPersistenceConfigurationItem : PersistenceConfigurationItem<SqlServerConfigurationParameters>
    {
        public SqlServerPersistenceConfigurationItem(SqlServerConfigurationParameters configurationParameters) 
            : base(configurationParameters)
        {
        }

        protected override IPersistenceConfigurer SetupDatabaseConnection()
        {
            var persistanceConfiguration = MsSqlConfiguration.MsSql2008
                .ConnectionString(ConfigurationParameters.ConnectionString);

            if (ConfigurationParameters.EnableQueryLogging)
            {
                persistanceConfiguration.ShowSql();
                persistanceConfiguration.FormatSql();
            }
            return persistanceConfiguration;
        }
    }
}