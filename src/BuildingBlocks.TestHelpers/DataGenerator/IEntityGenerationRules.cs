using AutoPoco.Configuration;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public interface IEntityGenerationRules<T>
    {
        void SetupGeneration(IEngineConfigurationTypeBuilder<T> typeBuilder);
    }
}