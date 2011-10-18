using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Configuration;

namespace BuildingBlocks.Persistence.Conventions.Configuration
{
    public class ConventionsConfiguration : IConfigurationItem
    {
        private readonly List<IConvention> _conventions;
        private MappingConventionBuilder _mappingConventionBuilder;
        private EntitiesExtentionsConventionBuilder _entitiesExtentionsConventionBuilder;

        public ConventionsConfiguration()
        {
            var entityMapConventions = new EntityMapConventions();
            var entityTitleConvention = new DefaultEntityTitleConvention();
            var entityCodeConvention = new DefaultEntityCodeConvention();
            _conventions = new List<IConvention> {entityMapConventions, entityTitleConvention, entityCodeConvention};

            CreateBuilders();
        }

        private ConventionsConfiguration(List<IConvention> conventions)
        {
            _conventions = conventions;

            CreateBuilders();
        }

        private void CreateBuilders()
        {
            _mappingConventionBuilder = new MappingConventionBuilder(_conventions);
            _entitiesExtentionsConventionBuilder = new EntitiesExtentionsConventionBuilder(_conventions);
        }

        public MappingConventionBuilder Mapping 
        {
            get { return _mappingConventionBuilder; }
        }

        public EntitiesExtentionsConventionBuilder EntitiesExtentions
        {
            get { return _entitiesExtentionsConventionBuilder; }
        }

        public ConventionsConfiguration Clone()
        {
            return new ConventionsConfiguration(_conventions.ToList());
        }

        public void Configure(IocContainer iocContainer)
        {
            var idConvention = GetFirstOfType<IEntityMapConventions>() ?? 
                 new EntityMapConventions();
            var titleConvention = GetFirstOfType<IEntityTitleConvention>()
                 ?? new NullEntityTitleConvention();
            var codeConvention = GetFirstOfType<IEntityCodeConvention>()
                 ?? new DefaultEntityCodeConvention();

            iocContainer.IocContainerConfiguration
                .For<IEntityMapConventions>()
                .Singleton()
                .Use(idConvention);
            iocContainer.IocContainerConfiguration
                .For<IEntityTitleConvention>()
                .Singleton()
                .Use(titleConvention);
            iocContainer.IocContainerConfiguration
                .For<IEntityCodeConvention>()
                .Singleton()
                .Use(codeConvention);
        }

        private T GetFirstOfType<T>()
            where T : IConvention
        {
            return _conventions.OfType<T>().FirstOrDefault();
        }
    }
}