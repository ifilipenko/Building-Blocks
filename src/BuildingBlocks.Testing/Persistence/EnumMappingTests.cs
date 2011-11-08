using System.Linq;
using BuildingBlocks.Persistence;
using BuildingBlocks.TestHelpers;
using BuildingBlocks.Testing.Persistence.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence
{
    [TestClass]
    public class EnumMappingTests : TestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            using (var uow = UnitOfWork.TransactionScope())
            {
                var category1 = new ProductCategoryEntity {Code = 10, Name = "Electro", ShortName = "E"};
                var category2 = new ProductCategoryEntity {Code = 20, Name = "Food", ShortName = "F"};
                var category3 = new ProductCategoryEntity {Code = 30, Name = "Stationery", ShortName = "S"};

                var repository = new Repository();
                repository.Save(category1);
                repository.Save(category2);
                repository.Save(category3);
                uow.SubmitChanges();
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            using (var uow = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                repository.Delete("from " + typeof(object));
                uow.SubmitChanges();
            }
        }

        [TestMethod]
        public void entity_with_enum_property_mapped_as_entityenum_should_correctly_mapped()
        {
            var product = new Product {Name = "Product", Category = ProductCategory.Food};

            using (var uow = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                repository.Save(product);
                uow.SubmitChanges();
            }

            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var loadedProduct = repository.GetByID<Product>(product.Id);
                loadedProduct.Should().NotBeNull();
                loadedProduct.Category.Should().Be(product.Category);
            }
        }

        [TestMethod]
        public void entity_with_enum_property_mapped_as_entityenum_should_store_mapped_values_in_db()
        {
            var storedValueType = typeof (ProductCategoryEntity).GetProperty("Code").PropertyType;
            var product = new Product {Name = "Product", Category = ProductCategory.Food};
            ProductCategoryEntity categoryEntity;

            using (var uow = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                categoryEntity = repository.Query<ProductCategoryEntity>()
                    .First(p => p.Name == product.Category.ToString());
                repository.Save(product);
                uow.SubmitChanges();
            }

            using (var uow = UnitOfWork.Scope())
            {
                var productCategoryStoredInDb = uow.Session.CreateSQLQuery("select p.Category from Product p where p.Id=" + product.Id).UniqueResult();
                productCategoryStoredInDb
                    .Should().BeOfType(storedValueType);
                ((long) productCategoryStoredInDb)
                    .Should().NotBe((long) ProductCategory.Food);
                categoryEntity.ID
                    .Should().Be((long) productCategoryStoredInDb);
            }
        }

        [TestMethod]
        public void should_persist_null_foreignkey_for_null_enum()
        {
            var product = new Product {Name = "Product", Category = ProductCategory.Undefined};

            using (var uow = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                repository.Save(product);
                uow.SubmitChanges();
            }

            using (var uow = UnitOfWork.Scope())
            {
                var persistedProductCategory = uow.Session
                    .CreateSQLQuery("select p.Category from Product p where p.Id=" + product.Id)
                    .UniqueResult();
                persistedProductCategory
                    .Should().BeNull();
            }

            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var loadedProduct = repository.GetByID<Product>(product.Id);
                loadedProduct.Category
                    .Should().Be(ProductCategory.Undefined);
            }
        }

        [TestMethod]
        public void should_filter_by_enum_values()
        {
            var foodProduct = new Product { Name = "Product1", Category = ProductCategory.Food };
            var electroProduct = new Product { Name = "Product2", Category = ProductCategory.Electro };

            using (var uow = UnitOfWork.TransactionScope())
            {
                var repository = new Repository();
                repository.Save(foodProduct);
                repository.Save(electroProduct);
                uow.SubmitChanges();
            }

            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                repository.Query<Product>()
                    .FirstOrDefault(p => p.Category == ProductCategory.Food)
                    .Should().NotBeNull();
            }
        }
    }
}