using System;
using System.Collections.Generic;
using BuildingBlocks.TestHelpers.DataGenerator;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    public interface IEnumEntitiesGenerationRules
    {
        TestDataPersistor Db { get; }
        DataGenerator DataGenerator { get; }

        void SetEntityPropertiesForEnum<TEntity, TEnum>(TEntity entity, TEnum enumValue)
            where TEnum : struct;

        IEnumerable<TEntity> CreateEntitiesForAllEnums<TEntity, TEnum>(Action<TEntity> entitySetup = null)
            where TEnum : struct
            where TEntity : new();
    }
}