using BuildingBlocks.Persistence;

namespace BuildingBlocks.Testing.Persistence.Model
{
    public class ProductCategoryEntity : ICacheableEntity
    {
        public virtual long ID { get; set; }
        public virtual long Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string ShortName { get; set; }
    }
}