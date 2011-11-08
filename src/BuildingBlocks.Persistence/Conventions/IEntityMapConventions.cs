namespace BuildingBlocks.Persistence.Conventions
{
    public interface IEntityMapConventions : IConvention
    {
        IdConvention IdConvention { get; set; }
        bool DefaultCacheable { get; set; }
    }
}