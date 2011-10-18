using System.Data;
using BuildingBlocks.Persistence.Configuration;
using FluentNHibernate.Cfg.Db;
using NHibernate.Connection;

namespace BuildingBlocks.Persistence.SQLite
{
    public class InMemorySqlitePersistenceConfigurationItem : PersistenceConfigurationItem<InMemorySqliteConfigurationParameters>
    {
        public InMemorySqlitePersistenceConfigurationItem(InMemorySqliteConfigurationParameters configurationParameters) 
            : base(configurationParameters)
        {
            ConfigurationParameters.SchemaUpdateMode = SchemaUpdateMode.Recreate;
            ConfigurationParameters.EnableStatistics = true;
        }

        protected override IPersistenceConfigurer SetupDatabaseConnection()
        {
            return SQLiteConfiguration.Standard
                .InMemory()
                .Provider<SingletonConnectionProvider>()
                .ConnectionString("Data Source=:memory:;Version=3;New=False;Pooling=True;Max Pool Size=1;")
                .UseOuterJoin()
                .FormatSql()
                .ShowSql();
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class SingletonConnectionProvider : DriverConnectionProvider
        {
            private IDbConnection _connection;

            public override IDbConnection GetConnection()
            {
                return _connection ?? (_connection = base.GetConnection());
            }

            protected override void Dispose(bool isDisposing)
            {
                if (!isDisposing)
                {
                    if (_connection != null)
                        _connection.Close();
                    base.Dispose(false);
                }
            }

            public override void CloseConnection(IDbConnection conn)
            {
            }
        }
        // ReSharper restore ClassNeverInstantiated.Local
    }
}