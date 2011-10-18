using System;
using System.Collections.Generic;
using BuildingBlocks.Persistence.Exceptions;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    internal class EnumMapContainer
    {
        private readonly Dictionary<Type, EnumMapMetadata> _enumMaps;

        public EnumMapContainer(IEnumerable<Type> enumMaps)
        {
            _enumMaps = new Dictionary<Type, EnumMapMetadata>();
            foreach (var enumMapType in enumMaps)
            {
                if (enumMapType.BaseType == null || 
                    !(enumMapType.BaseType.IsGenericType && 
                    enumMapType.BaseType.GetGenericTypeDefinition() == typeof(EnumMap<,>)))
                    continue;
                var genericArgs = enumMapType.BaseType.GetGenericArguments();
                var enumType = genericArgs[0];
                var enumEntity = genericArgs[1];
                _enumMaps.Add(enumType, new EnumMapMetadata(enumType, enumEntity, enumMapType));
            }
        }

        public EnumMapMetadata GetEnumMapMetadata(Type enumType)
        {
            try
            {
                return _enumMaps[enumType];
            }
            catch (KeyNotFoundException)
            {
                throw new EnumToEntityMapException(string.Format("Enum map for enum {0} is not exists", enumType));
            }
        }
    }
}