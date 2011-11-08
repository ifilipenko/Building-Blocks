using BuildingBlocks.Persistence.Mapping;
using BuildingBlocks.Testing.Persistence.Model;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence
{
    [TestClass]
    public class EntityMappingTests : TestBase
    {
        class ProductMap : EntityMap<Product>
        {
            public ProductMap()
            {
                Table("Product");

                Id(s => s.Name);

                Map(s => s.Category);
            }
        }

        [TestMethod]
        public void should_able_map_entity_with_string_id()
        {
            var sqLiteConfiguration = SQLiteConfiguration.Standard
                .InMemory()
                .ConnectionString("Data Source=:memory:;Version=3;New=False;Pooling=True;Max Pool Size=1;")
                .UseOuterJoin()
                .FormatSql()
                .ShowSql();
            Fluently.Configure()
                .Database(sqLiteConfiguration)
                .Mappings(m => m.FluentMappings.Add<ProductMap>())
                .BuildSessionFactory();
        }
    }
}