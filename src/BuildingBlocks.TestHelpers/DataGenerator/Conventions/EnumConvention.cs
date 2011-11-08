using AutoPoco.Configuration;
using BuildingBlocks.TestHelpers.DataGenerator.DataSources;

namespace BuildingBlocks.TestHelpers.DataGenerator.Conventions
{
    public class EnumConvention : ITypeFieldConvention, ITypePropertyConvention
    {
        public void SpecifyRequirements(ITypeMemberConventionRequirements requirements)
        {
            requirements.Type(x => x.IsEnum);
        }

        public void Apply(ITypePropertyConventionContext context)
        {
            if (!context.Member.PropertyInfo.PropertyType.IsEnum)
                return;
            context.SetSource<RandomEnumSource>();
        }

        public void Apply(ITypeFieldConventionContext context)
        {
            if (!context.Member.FieldInfo.FieldType.IsEnum)
                return;
        }
    }
}