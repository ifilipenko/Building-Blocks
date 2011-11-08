using System.Linq;
using BuildingBlocks.Persistence;
using BuildingBlocks.Testing.Persistence.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Testing.Persistence
{
    [TestClass]
    public class EnumRepositoryTests : TestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            using (var uow = UnitOfWork.TransactionScope())
            {
                var category1 = new ProductCategoryEntity { Code = 10, Name = "Electro", ShortName = "E" };
                var category2 = new ProductCategoryEntity { Code = 20, Name = "Food", ShortName = "F" };
                var category3 = new ProductCategoryEntity { Code = 30, Name = "Stationery", ShortName = "S" };

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
        public void should_return_all_enum_values()
        {
            using (UnitOfWork.Scope())
            {
                var enumRepository = new EnumRepository();
                var enumEntities = enumRepository.GetAllEntitiesForEnum<ProductCategory>();
                enumEntities
                    .Should().HaveCount(3);
                enumEntities.All(e => Equals(e["$type$"], typeof (ProductCategoryEntity).FullName)).Should().BeTrue();
            }
        }

        [TestMethod]
        public void should_return_entity_by_enum()
        {
            using (UnitOfWork.Scope())
            {
                var enumRepository = new EnumRepository();
                var food = enumRepository.GetEntityForEnum(ProductCategory.Food);
                food.Should().NotBeNull();
                food["Name"].Should().Be(ProductCategory.Food.ToString());
            }
        }

        [TestMethod]
        public void should_return_entity_id_by_enum()
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var expectedId =repository.Query<ProductCategoryEntity>()
                    .Where(c => c.Name == ProductCategory.Food.ToString())
                    .Select(c => c.ID)
                    .First();

                var enumRepository = new EnumRepository();
                var actualId = enumRepository.GetEntityIdForEnum(ProductCategory.Food);

                actualId.Should().Be(expectedId);
            }
        }

        [TestMethod]
        public void should_return_entity_title_by_enum()
        {
            using (UnitOfWork.Scope())
            {
                var enumRepository = new EnumRepository();
                var title = enumRepository.GetEntityTitleForEnum(ProductCategory.Food);

                title.Should().Be(ProductCategory.Food.ToString());
            }
        }

        [TestMethod]
        public void should_return_entity_titles()
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var expectedTitles =repository
                    .Query<ProductCategoryEntity>()
                    .OrderBy(c => c.Name)
                    .Select(c => c.Name)
                    .ToList();

                var enumRepository = new EnumRepository();
                var titles = enumRepository.GetEntityTitlesForEnum<ProductCategory>();

                titles.OrderBy(t => t).SequenceEqual(expectedTitles).Should().BeTrue();
            }
        }
    }
}