using BuildingBlocks.Persistence.Configuration;

namespace BuildingBlocks.Persistence.SQLite
{
    public class SqlLiteFileParameters : PersistenceConfigurationParameters<SqlLiteFileParameters>
    {
        private const string DefaultDbFilePath = "ScheduleDatabase";

        public SqlLiteFileParameters()
        {
            DbFilePath = DefaultDbFilePath;
        }

        public  string DbFilePath { get; set; }

        public override SqlLiteFileParameters Clone()
        {
            var result = base.Clone();
            result.DbFilePath = DbFilePath;
            return result;
        }
    }
}