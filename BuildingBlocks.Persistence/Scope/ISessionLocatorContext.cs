namespace BuildingBlocks.Persistence.Scope
{
    public interface ISessionLocatorContext
    {
        SessionLocatorItem Item { get; set; }
        void RemoveItem();
    }
}