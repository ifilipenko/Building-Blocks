namespace BuildingBlocks.Persistence
{
    public enum EntityPersistentState
    {
        New,
        Deleting,
        DeleteCompleted,
        Loaded,
        Saved,
        Updated
    }
}