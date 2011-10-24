using BuildingBlocks.Persistence.Mapping;
using BuildingBlocks.Persistence.Mapping.EnumMap;
using BuildingBlocks.Testing.Persistence.Model;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Testing.Persistence.Maps
{
    public class ProductMaps : EntityMap<Product>
    {
        public ProductMaps()
        {
            Table("Product");

            Id(p => p.Id);
            Map(p => p.Name);
            EnumMap(p => p.Category, "Category",
                    m => m.ByEnumMap(new ProductCategoryEnumMap()));
            //b => b.MapEnumFrom<ProductCategoryEntity>()
            //         .EntityId(e => e.ID)
            //         .MapValuesBy(x => x.Code)
            //         .For(10).Enum(ProductCategory.Electro)
            //         .For(20).Enum(ProductCategory.Food)
            //         .For(30).Enum(ProductCategory.Stationery)
            //         .ForNull.Enum(ProductCategory.Undefined));
        }
    }

    public class ProductCategoryEnumMap : EnumMap<ProductCategory, ProductCategoryEntity>
    {
        public ProductCategoryEnumMap()
        {
            EntityId(p => p.ID);
            MapValuesBy(x => x.Code)
                .For(10).Enum(ProductCategory.Electro)
                .For(20).Enum(ProductCategory.Food)
                .For(30).Enum(ProductCategory.Stationery)
                .ForNull.Enum(ProductCategory.Undefined);
        }
    }

    public class ProductCategoryEntityMap : ClassMap<ProductCategoryEntity>
    {
        public ProductCategoryEntityMap()
        {
            Table("ProductCategoryEntity");

            Id(x => x.ID);
            Map(p => p.Code);
            Map(p => p.Name);
            Map(p => p.ShortName);
        }
    }
}