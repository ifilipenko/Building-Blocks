using AutoPoco.Configuration;

namespace BuildingBlocks.Autopoco.Helpers
{
    public interface IPocoBuilder<TPoco>
    {
        void Setup(IEngineConfigurationTypeBuilder<TPoco> setup);
    }
}