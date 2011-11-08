using System;
using System.Collections.Generic;
using BuildingBlocks.TestHelpers.DataGenerator;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    public class EnumEntitiesGenerationRules : IEnumEntitiesGenerationRules
    {
        private readonly DataGenerator _dataGenerator;
        private readonly TestDataPersistor _db;
        private readonly IEnumRepository _enumRepository;

        public EnumEntitiesGenerationRules(DataGenerator dataGenerator, TestDataPersistor db)
        {
            _dataGenerator = dataGenerator;
            _db = db;
            _enumRepository = new EnumRepository();
        }

        public TestDataPersistor Db
        {
            get { return _db; }
        }

        public DataGenerator DataGenerator
        {
            get { return _dataGenerator; }
        }

        public void SetEntityPropertiesForEnum<TEntity, TEnum>(TEntity entity, TEnum @enum)
            where TEnum : struct
        {
            _enumRepository.SetEnumEntityValueForEnum(entity, @enum);
        }

        public IEnumerable<TEntity> CreateEntitiesForAllEnums<TEntity, TEnum>(Action<TEntity> entitySetup = null) 
            where TEnum : struct
            where TEntity : new()
        {
            foreach (TEnum @enum in System.Enum.GetValues(typeof(TEnum)))
            {
                var entity = new TEntity();
                _enumRepository.SetEnumEntityTitle<TEnum>(entity, @enum.ToString());
                if (entitySetup != null)
                {
                    entitySetup(entity);
                }
                _enumRepository.SetEnumEntityValueForEnum(entity, @enum);
                yield return entity;
            }
        }
    }
}