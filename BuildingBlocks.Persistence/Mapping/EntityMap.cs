using System;
using System.Linq.Expressions;
using BuildingBlocks.Persistence.Conventions;
using BuildingBlocks.Persistence.Exceptions;
using BuildingBlocks.Persistence.Mapping.EnumMap;
using CuttingEdge.Conditions;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Utils;

namespace BuildingBlocks.Persistence.Mapping
{
    public class EntityMap<T> : ClassMap<T>
    {
        private readonly AttributeStore<ClassMapping> _attributeStore;
        private readonly MappingProviderStore _mappingProviderStore;
        private readonly IEntityMapConventions _conventions;

        public EntityMap() 
            : this(new AttributeStore<ClassMapping>(), new MappingProviderStore())
        {
        }

        public EntityMap(AttributeStore<ClassMapping> attributeStore, MappingProviderStore mappingProviderStore)
            : base(attributeStore, mappingProviderStore)
        {
            _attributeStore = attributeStore;
            _mappingProviderStore = mappingProviderStore;

            _conventions = PersistenceEnvironment.ConventionsLocator.MapConventions;

            if (_conventions.DefaultCacheable || typeof(ICacheableEntity).IsAssignableFrom(typeof(T)))
            {
                Cache.ReadWrite();
            }
        }

        //public PropertyPart Map(Expression<Func<T, bool>> memberExpression, string column = null)
        //{
        //    var propertyPart = Map(memberExpression.ToMember(), column);
        //    if (_conventions.BooleanConvention != null)
        //    {
        //        _conventions.BooleanConvention.CustomPropertyType
        //    }
        //    return propertyPart
        //        .CustomType<BoolType>();
        //}

        public override IdentityPart Id(Expression<Func<T, object>> memberExpression, string column)
        {
            column = column ?? memberExpression.ToMember().Name;
            var identityPart = base.Id(memberExpression, column);
            SetupIdentityGenerator(identityPart);
            return identityPart;
        }

        public EnumMapPropertyPart EnumMap<TEnum>(Expression<Func<T, TEnum>> memberExpression, string columnName, Action<EnumMapBuilder<TEnum>> action) 
            where TEnum : struct
        {
            Condition.Requires(memberExpression, "memberExpression").IsNotNull();

            var member = memberExpression.ToMember();
            return EnumMapCore(member, columnName, action, false);
        }

        public EnumMapPropertyPart EnumMap<TEnum>(Expression<Func<T, TEnum?>> memberExpression, string columnName, Action<EnumMapBuilder<TEnum>> action)
            where TEnum : struct
        {
            Condition.Requires(memberExpression, "memberExpression").IsNotNull();

            var member = memberExpression.ToMember();
            return EnumMapCore(member, columnName, action, true);
        }

        public EnumMapPropertyPart EnumMap<TEnum>(Expression<Func<T, TEnum>> memberExpression, Action<EnumMapBuilder<TEnum>> action)
            where TEnum : struct
        {
            Condition.Requires(memberExpression, "memberExpression").IsNotNull();

            var member = memberExpression.ToMember();
            return EnumMapCore(member, null, action, false);
        }
        
        public EnumMapPropertyPart EnumMap<TEnum>(Expression<Func<T, TEnum?>> memberExpression, Action<EnumMapBuilder<TEnum>> action)
            where TEnum : struct
        {
            Condition.Requires(memberExpression, "memberExpression").IsNotNull();

            var member = memberExpression.ToMember();
            return EnumMapCore(member, null, action, true);
        }

        private EnumMapPropertyPart EnumMapCore<TEnum>(Member member, string columnName, Action<EnumMapBuilder<TEnum>> action, bool nullable) 
            where TEnum : struct
        {
            Condition.Requires(action, "action").IsNotNull();

            var enumMapBuilder = new EnumMapBuilder<TEnum>();
            action(enumMapBuilder);

            if (enumMapBuilder.ResultEnumMap == null)
            {
                throw new PersistenceException("Enum mapping is not set");
            }

            var propertyPart = Map(member, columnName);
            enumMapBuilder.ResultEnumMap.CreateRulesForProperty<T>(propertyPart);

            if (nullable)
            {
                propertyPart.Nullable();
            }

            return new EnumMapPropertyPart(propertyPart);
        }

        private void SetupIdentityGenerator(IdentityPart identityPart)
        {
            if (identityPart == null)
                return;

            if (_conventions == null || _conventions.IdConvention == null)
            {
                identityPart.GeneratedBy.Native();
                return;
            }

            var tableName = _attributeStore.Get(c => c.TableName);
            _conventions.IdConvention(identityPart, tableName);
        }
    }

    public class BoolType
    {
    }
}