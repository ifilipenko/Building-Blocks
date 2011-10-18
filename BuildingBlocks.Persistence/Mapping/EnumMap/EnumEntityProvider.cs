using System;
using System.Collections;
using System.Linq;
using BuildingBlocks.Persistence.Scope;
using NHibernate;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    class EnumEntityProvider
    {
        private readonly SessionLocator _sessionLocator;

        public EnumEntityProvider()
            : this(SessionLocator.Get())
        {
        }

        public EnumEntityProvider(SessionLocator sessionLocator)
        {
            _sessionLocator = sessionLocator;
        }

        public object GetEnumEntityIdByEnum(object enumValue, EnumToEnumEntityConvertionRule enumToEnumEntityConvertionRule)
        {
            var entityValue = enumToEnumEntityConvertionRule.ToEntityValue((System.Enum)enumValue);
            var allEnumEntity = GetAllEnumEntity(enumToEnumEntityConvertionRule);
            
            var property = enumToEnumEntityConvertionRule.ResultProperty;
            var enumEntity = allEnumEntity
                .OfType<Hashtable>()
                .FirstOrDefault(e => Equals(e[property.Name], entityValue));

            if (enumEntity == null)
                return null;

            var idPropName = GetIdentifierName(enumToEnumEntityConvertionRule);
            var id = enumEntity[idPropName];
            return id;
        }

        public object GetEnumById(object id, EnumToEnumEntityConvertionRule enumToEnumEntityConvertionRule)
        {
            if (id == null || id == DBNull.Value)
                return null;

            var idPropName = GetIdentifierName(enumToEnumEntityConvertionRule);
            var allEnumEntity = GetAllEnumEntity(enumToEnumEntityConvertionRule);
            var entity = allEnumEntity
                .OfType<Hashtable>()
                .FirstOrDefault(e => Equals(e[idPropName], id));
            if (entity == null)
                return null;

            var valuePropertyName = enumToEnumEntityConvertionRule.ResultProperty.Name;
            var enumEntityValue = entity[valuePropertyName];
            return enumToEnumEntityConvertionRule.ToEnum(enumEntityValue);
        }

        public IList GetAllEnumEntity(Type enumEntity)
        {
            using (var session = _sessionLocator.SessionFactory.OpenSession())
            {
                var criteria = session
                    .GetSession(EntityMode.Map)
                    .CreateCriteria(enumEntity);
                return criteria
                    .SetCacheable(true)
                    .SetCacheMode(CacheMode.Normal)
                    .List();
            }
        }

        private IList GetAllEnumEntity(EnumToEnumEntityConvertionRule enumToEnumEntityConvertionRule)
        {
            return GetAllEnumEntity(enumToEnumEntityConvertionRule.EnumEntity);
        }

        private string GetIdentifierName(EnumToEnumEntityConvertionRule enumToEnumEntityConvertionRule)
        {
            return _sessionLocator
                .SessionFactory
                .GetClassMetadata(enumToEnumEntityConvertionRule.EnumEntity)
                .IdentifierPropertyName;
        }
    }
}