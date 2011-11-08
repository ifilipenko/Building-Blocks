using System.Collections;
using System.Collections.Generic;
using BuildingBlocks.Persistence.Enum;

namespace BuildingBlocks.Persistence
{
    public interface IEnumRepository
    {
        IEnumerable<Hashtable> GetAllEntitiesForEnum<TEnum>()
            where TEnum : struct;

        IEnumerable<CommonEnumEntity<TEnum>> GetCommonEntitiesFor<TEnum>()
            where TEnum : struct;

        CommonEnumEntity<TEnum> GetCommonEntityForEnum<TEnum>(TEnum @enum)
            where TEnum : struct;

        Hashtable GetEntityForEnum<TEnum>(TEnum @enum)
            where TEnum : struct;

        TEnum GetEnumById<TEnum>(object id)
            where TEnum : struct;

        TEnum GetEnumByTitle<TEnum>(string title)
            where TEnum : struct;

        object GetEntityIdForEnum<TEnum>(TEnum @enum)
            where TEnum : struct;

        string GetEntityTitleForEnum<TEnum>(TEnum @enum)
            where TEnum : struct;

        IEnumerable<string> GetEntityTitlesForEnum<TEnum>()
            where TEnum : struct;

        void SetEnumEntityValueForEnum<TEnum>(object enumEntity, TEnum @enum)
            where TEnum : struct;

        void SetEnumEntityTitle<TEnum>(object enumEntity, string title)
            where TEnum : struct;
    }
}