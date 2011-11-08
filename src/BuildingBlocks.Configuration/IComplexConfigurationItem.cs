namespace BuildingBlocks.Configuration
{
    public interface IComplexConfigurationItem : IConfigurationItem
    {
        void IncludeItems(IComplexConfigurationItemsList itemsList);
    }
}