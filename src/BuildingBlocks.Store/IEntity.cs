namespace BuildingBlocks.Store
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }
    }
}