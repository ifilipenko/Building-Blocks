namespace BuildingBlocks.Configuration
{
    public interface IConfigurationItem
    {
        void Configure(IocContainer iocContainer);
    }
}