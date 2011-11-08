using System;
using BuildingBlocks.Configuration;
using BuildingBlocks.Persistence.Configuration;
using BuildingBlocks.Persistence.SQLite;
using BuildingBlocks.Testing.Persistence.Maps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence
{
    [TestClass]
    public class TestBase
    {
        static TestBase()
        {
            var parameters = new SqlLiteFileParameters
            {
                DbFilePath = "test.db",
                MappingAssemblies = new[] { typeof(TariffPlanMap).Assembly },
                SchemaOutput = cmd => Console.WriteLine(cmd),
                SchemaUpdateMode = SchemaUpdateMode.Recreate,
                EnableStatistics = true
            };
            var persistenceConfigurationItem = new SqliteFilePersistenceConfigurationItem(parameters);

            var configurator = new Configurator();
            configurator.AddItem(persistenceConfigurationItem);
            configurator.Configure();
        }
    }
}