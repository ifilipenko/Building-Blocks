using System.Collections.Generic;

namespace BuildingBlocks.Persistence.Conventions.Configuration
{
    public class EntitiesExtentionsConventionBuilder
    {
        private readonly List<IConvention> _conventions;

        public EntitiesExtentionsConventionBuilder(List<IConvention> conventions)
        {
            _conventions = conventions;
        }

        public void SetTitleConventions(IEntityTitleConvention titleConvention)
        {
            _conventions.RemoveAll(c => c is IEntityTitleConvention);
            if (titleConvention != null)
            {
                _conventions.Add(titleConvention);
            }
        }
    }
}