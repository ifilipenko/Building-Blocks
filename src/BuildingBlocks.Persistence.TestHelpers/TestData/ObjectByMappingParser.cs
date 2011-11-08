using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;
using NHibernate;
using NHibernate.Type;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    class ObjectByMappingParser
    {
        private readonly ISessionFactory _sessionFactory;
        private List<object> _parsedObjects;

        public ObjectByMappingParser(ISessionFactory sessionFactory)
        {
            Condition.Requires(sessionFactory, "sessionFactory").IsNotNull();
            _sessionFactory = sessionFactory;
        }

        public ObjectParsedValues ParseObject(object obj)
        {
            Condition.Requires(obj, "obj").IsNotNull();

            _parsedObjects = new List<object>();

            var classMetadata = _sessionFactory.GetClassMetadata(obj.GetType());
            if (classMetadata == null)
            {
                throw new InvalidOperationException("Class [" + obj.GetType() + "] is not mapped");
            }

            var parsedValues = new ObjectParsedValues();
            ParseObjectProperties(obj, new EntityMappingMetadataProvider(classMetadata), parsedValues);

            _parsedObjects.Clear();
            return parsedValues;
        }

        private void ParseObjectProperties(object obj, IMappingMetadataProvider metadata, ObjectParsedValues parsedValues)
        {
            if (_parsedObjects.Any(o => ReferenceEquals(o, obj)))
                return;

            _parsedObjects.Add(obj);

            var values = metadata.GetPropertyValues(obj);
            var propertyNames = metadata.GetPropertyNames();
            var propertyTypes = metadata.GetPropertyTypes();
            for (int i = 0; i < propertyNames.Length; i++)
            {
                var type = propertyTypes[i];
                var value = new ObjectValue
                {
                    Value = values[i],
                    Property = propertyNames[i]
                };
                if (type.IsAssociationType && type.IsEntityType)
                {
                    parsedValues.AddAssociation(value);
                }
                else if (type.IsComponentType)
                {
                    parsedValues.AddComponents(value);
                    ParseObjectProperties(value.Value, new ComponentMappingMetadataProvider((ComponentType) type), parsedValues);
                }
                else if (type.IsAssociationType && type.IsCollectionType)
                {
                    parsedValues.AddCollection(value);
                    ParseCollectionItems(value, parsedValues);
                }
                else
                {
                    parsedValues.AddPrimitive(value);
                }
            }
        }

        private void ParseCollectionItems(ObjectValue value, ObjectParsedValues parsedValues)
        {
            var items = value.Value as IEnumerable;
            if (items == null)
                return;

            foreach (var item in items)
            {
                var metadata = _sessionFactory.GetClassMetadata(item.GetType());
                if (metadata == null)
                    continue;
                ParseObjectProperties(item, new EntityMappingMetadataProvider(metadata), parsedValues);
            }
        }
    }
}