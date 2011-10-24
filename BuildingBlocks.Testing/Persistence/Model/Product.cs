namespace BuildingBlocks.Testing.Persistence.Model
{
    public class Product
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ProductCategory Category { get; set; }
    }
}