using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common.Sugar;
using BuildingBlocks.Persistence.Enum;
using BuildingBlocks.Persistence.Exceptions;
using BuildingBlocks.Persistence.Mapping.EnumMap;
using BuildingBlocks.Persistence.Scope;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Persistence
{
    public class EnumRepository : IEnumRepository
    {
        private readonly EnumMapContainer _enumMapContainer;
        private readonly EnumEntityProvider _provider;

        public EnumRepository()
        {
            _enumMapContainer = PersistenceEnvironment.GetEnumMapContainer();
            var _sessionLocator = SessionLocator.Get();
            _provider = new EnumEntityProvider(_sessionLocator);
        }

        public IEnumerable<Hashtable> GetAllEntitiesForEnum<TEnum>()
            where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            return GetAllEnumEntity(enumMapMetadata).ToList();
        }

        public IEnumerable<CommonEnumEntity<TEnum>> GetCommonEntitiesFor<TEnum>()
            where TEnum : struct
        {
            var entities = GetCommonEnumEntitiesCore<TEnum>().ToList();
            return entities;
        }

        public CommonEnumEntity<TEnum> GetCommonEntityForEnum<TEnum>(TEnum @enum) 
            where TEnum : struct
        {
            return GetCommonEnumEntitiesCore<TEnum>().FirstOrDefault(e => Equals(e.Enum, @enum));
        }

        public Hashtable GetEntityForEnum<TEnum>(TEnum @enum)
            where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            var entity = GetEntityForEnum(enumMapMetadata, @enum);
            return entity;
        }

        public TEnum GetEnumById<TEnum>(object id)
           where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            var entities = GetAllEnumEntity(enumMapMetadata);
            var entity = entities
                .FirstOrDefault(e => Equals(e[enumMapMetadata.EnumEntityIdProperty], id));
            if (entity == null)
                throw new PersistenceException(string.Format("Enum entity with id {0} is not exists", id));

            var value = entity[enumMapMetadata.EnumEntityValueProperty];
            var @enum = enumMapMetadata.GetEnumByEnumEntityValue(value);

            return (TEnum) @enum;
        }

        public TEnum GetEnumByTitle<TEnum>(string title)
           where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            var entities = GetAllEnumEntity(enumMapMetadata);
            var titleProperty = GetTitleProperty(enumMapMetadata);
            var entity = entities
                .FirstOrDefault(e => Equals(e[titleProperty], title));
            if (entity == null)
                throw new PersistenceException(string.Format("Enum entity with title {0} is not exists", title));

            var value = entity[enumMapMetadata.EnumEntityValueProperty];
            var @enum = enumMapMetadata.GetEnumByEnumEntityValue(value);

            return (TEnum)@enum; 
        }

        public object GetEntityIdForEnum<TEnum>(TEnum @enum)
            where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            var entity = GetEntityForEnum(enumMapMetadata, @enum);
            return entity[enumMapMetadata.EnumEntityIdProperty];
        }

        public string GetEntityTitleForEnum<TEnum>(TEnum @enum)
            where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            var titleProperty = GetTitleProperty(enumMapMetadata);
            var entity = GetEntityForEnum(enumMapMetadata, @enum);
            return (entity[titleProperty] ?? string.Empty).ToString();
        }

        public IEnumerable<string> GetEntityTitlesForEnum<TEnum>()
            where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            var titleProperty = GetTitleProperty(enumMapMetadata);
            var entities = GetAllEnumEntity(enumMapMetadata);
            return entities
                .Select(e => (e[titleProperty] ?? string.Empty).ToString())
                .ToList();
        }

        public void SetEnumEntityValueForEnum<TEnum>(object enumEntity, TEnum @enum)
            where TEnum : struct
        {
            Condition.Requires(enumEntity, "enumEntity").IsNotNull();

            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            if (!enumMapMetadata.EnumEntity.IsInstanceOfType(enumEntity))
                throw new ArgumentException(string.Format("Expected entity of type \"{0}\" but was \"{1}\"",
                                                          enumMapMetadata.EnumEntity,
                                                          enumMapMetadata.GetType()));
            var value = enumMapMetadata.GetEnumEntityValueForEnum(@enum);
            var propertyInfo = enumEntity.GetType().GetProperty(enumMapMetadata.EnumEntityValueProperty);
            propertyInfo.SetValue(enumEntity, value, null);
        }

        public void SetEnumEntityTitle<TEnum>(object enumEntity, string title)
            where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            if (!enumMapMetadata.EnumEntity.IsInstanceOfType(enumEntity))
                throw new ArgumentException(string.Format("Expected entity of type \"{0}\" but was \"{1}\"",
                                                          enumMapMetadata.EnumEntity,
                                                          enumMapMetadata.GetType()));

            var titleProperty = GetTitleProperty(enumMapMetadata);
            var propertyInfo = enumEntity.GetType().GetProperty(titleProperty);
            propertyInfo.SetValue(enumEntity, title, null);
        }

        private string GetTitleProperty(EnumMapMetadata enumMapMetadata)
        {
            var titleProperty = GetTitlePropertyOrNull(enumMapMetadata);
            if (titleProperty == null)
            {
                var message = string.Format(
                    "expected that the type \"{0}\" has a property title entity, but all of the properties of this type do not match title entity convention",
                    enumMapMetadata.EnumEntity);
                throw new EnumToEntityMapException(message);
            }

            return titleProperty;
        }

        private string GetTitlePropertyOrNull(EnumMapMetadata enumMapMetadata)
        {
            var entityTitleConvention = PersistenceEnvironment.ConventionsLocator.TitleConvention;
            var titleProperty = entityTitleConvention.ApplyTo(enumMapMetadata.EnumEntity);
            return titleProperty == null ? null : titleProperty.Name;
        }

        private string GetCodePropertyOrNull(EnumMapMetadata enumMapMetadata)
        {
            var entityTitleConvention = PersistenceEnvironment.ConventionsLocator.CodeConvention;
            var codeProperty = entityTitleConvention.ApplyTo(enumMapMetadata.EnumEntity);
            return codeProperty == null ? null : codeProperty.Name;
        }

        private Hashtable GetEntityForEnum<TEnum>(EnumMapMetadata enumMapMetadata, TEnum @enum)
            where TEnum : struct
        {
            var valueProperty = enumMapMetadata.EnumEntityValueProperty;
            var value = enumMapMetadata.GetEnumEntityValueForEnum(@enum);
            var entities = GetAllEnumEntity(enumMapMetadata);
            return entities.FirstOrDefault(e => Equals(e[valueProperty], value));
        }

        private EnumMapMetadata GetEnumMapMetadata<TEnum>()
            where TEnum : struct
        {
            var enumMapMetadata = _enumMapContainer.GetEnumMapMetadata(typeof(TEnum));
            if (enumMapMetadata == null)
            {
                throw new EnumToEntityMapException("Enum has no map");
            }
            return enumMapMetadata;
        }

        private IEnumerable<CommonEnumEntity<TEnum>> GetCommonEnumEntitiesCore<TEnum>() 
            where TEnum : struct
        {
            var enumMapMetadata = GetEnumMapMetadata<TEnum>();
            var titleProperty = GetTitlePropertyOrNull(enumMapMetadata);
            var codeProperty = GetCodePropertyOrNull(enumMapMetadata);

            var entities = GetAllEnumEntity(enumMapMetadata)
                .Select(e =>
                {
                    var code = codeProperty == null ? null : e[codeProperty];
                    var title = titleProperty == null ? null : e[titleProperty].ConvertToString();
                    var value = e[enumMapMetadata.EnumEntityValueProperty];
                    var id = e[enumMapMetadata.EnumEntityIdProperty];
                    var @enum = (TEnum)enumMapMetadata.GetEnumByEnumEntityValue(value);
                    return new CommonEnumEntity<TEnum>
                    {
                        Code = code,
                        Enum = @enum,
                        Id = id,
                        ResolveKey = value,
                        Title = title
                    };
                });
            return entities;
        }

        private IEnumerable<Hashtable> GetAllEnumEntity(EnumMapMetadata enumMapMetadata)
        {
            return _provider.GetAllEnumEntity(enumMapMetadata.EnumEntity).OfType<Hashtable>();
        }
    }
}