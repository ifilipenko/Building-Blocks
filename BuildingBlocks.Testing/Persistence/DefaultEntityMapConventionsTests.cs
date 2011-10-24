using BuildingBlocks.Common.Reflection;
using BuildingBlocks.Persistence.Conventions;
using BuildingBlocks.Testing.Persistence.Model;
using FluentAssertions;
using FluentNHibernate.Mapping;
using FluentNHibernate.Mapping.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence
{
    [TestClass]
    public class DefaultEntityMapConventionsTests
    {
        private EntityMapConventions _entityMapConventions;

        [TestInitialize]
        public void SetUp()
        {
            _entityMapConventions = new EntityMapConventions();
        }

        [TestMethod]
        public void should_use_Native_id_generator_for_integral_id()
        {
            var entityMap = new ClassMap<Product>();
            _entityMapConventions.IdConvention(entityMap.Id(s => s.Id), "table");
            var identityMappingProvider = entityMap.GetFieldValueAs<IIdentityMappingProvider>("id");
            var generator = identityMappingProvider.GetIdentityMapping().Generator;
            generator.Class
                .Should().Be("native");
            generator.Params["sequence"]
                .Should().Be("seq_table");
        }

        [TestMethod]
        public void should_use_Assigned_id_generator_for_string_id()
        {
            var entityMap = new ClassMap<Product>();
            _entityMapConventions.IdConvention(entityMap.Id(s => s.Name), "table");
            var identityMappingProvider = entityMap.GetFieldValueAs<IIdentityMappingProvider>("id");
            var generator = identityMappingProvider.GetIdentityMapping().Generator;
            generator.Class
                .Should().Be("assigned");
        }
    }
}