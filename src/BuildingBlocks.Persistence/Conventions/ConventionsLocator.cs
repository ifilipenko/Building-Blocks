using StructureMap;

namespace BuildingBlocks.Persistence.Conventions
{
    public class ConventionsLocator
    {
        public IEntityMapConventions MapConventions
        {
            get
            {
                return ObjectFactory.GetInstance<IEntityMapConventions>();
            }
        }

        public IEntityTitleConvention TitleConvention
        {
            get
            {
                return ObjectFactory.TryGetInstance<IEntityTitleConvention>();
            }
        }

        public IEntityCodeConvention CodeConvention
        {
            get
            {
                return ObjectFactory.TryGetInstance<IEntityCodeConvention>();
            }
        }
    }
}