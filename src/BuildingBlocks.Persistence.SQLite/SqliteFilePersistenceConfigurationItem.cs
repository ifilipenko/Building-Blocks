using BuildingBlocks.Persistence.Configuration;
using FluentNHibernate.Cfg.Db;

namespace BuildingBlocks.Persistence.SQLite
{
    public class SqliteFilePersistenceConfigurationItem : PersistenceConfigurationItem<SqlLiteFileParameters>
    {
        public SqliteFilePersistenceConfigurationItem(SqlLiteFileParameters configurationParameters)
            : base(configurationParameters)
        {
            configurationParameters.SchemaUpdateMode = SchemaUpdateMode.Update;
        }

        protected override IPersistenceConfigurer SetupDatabaseConnection()
        {
            return SQLiteConfiguration.Standard
                .UsingFile(ConfigurationParameters.DbFilePath)
                .UseOuterJoin()
                .FormatSql()
                .ShowSql();
        }
    }
}