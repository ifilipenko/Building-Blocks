using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Persistence.Conventions.Configuration
{
    public class MappingConventionBuilder
    {
        private readonly List<IConvention> _conventions;

        public MappingConventionBuilder(List<IConvention> conventions)
        {
            _conventions = conventions;
        }

        public MappingConventionBuilder SetIdConvention(IdConvention idConvention)
        {
            EnsureEntityMapConventions().IdConvention = idConvention;
            return this;
        }

        public MappingConventionBuilder SetDefaultCacheable(bool defaultCasheable)
        {
            EnsureEntityMapConventions().DefaultCacheable = defaultCasheable;
            return this;
        }

        private IEntityMapConventions EnsureEntityMapConventions()
        {
            var entityMapConventions = _conventions.FirstOrDefault(c => c is IEntityMapConventions) as IEntityMapConventions;
            if (entityMapConventions == null)
            {
                entityMapConventions = new EntityMapConventions();
                _conventions.Add(entityMapConventions);
            }
            return entityMapConventions;
        }
    }
}