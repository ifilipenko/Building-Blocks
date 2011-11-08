using System;
using System.Data;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    public class EnumToEnumEntityType<TEnum, TEnumOwner, TEnumEntity> : AbstractEnumType
    {
        private static EnumToEnumEntityConvertionRule GetRule()
        {
            var key = new EnumToEnumEntityRuleKey
            {
                Enum = typeof(TEnum),
                EnumEntity = typeof(TEnumEntity),
                SourceEntity = typeof(TEnumOwner)
            };
            return EnumToEnumEntityRuleLocator.Get().GetRule(key);
        }

        private readonly EnumToEnumEntityConvertionRule _convertionRule;
        private readonly SqlType _sqlType;

        public EnumToEnumEntityType()
            : base(SqlTypeFactory.Int64, typeof (TEnum))
        {
            _convertionRule = GetRule();
            _sqlType = SqlTypeConverter.GetSqlType(_convertionRule.EnumEntityIdentifierType);
        }

        public override object Assemble(object cached, ISessionImplementor session, object owner)
        {
            if (cached != null)
            {
                return GetInstance(cached);
            }
            return null;
        }

        public override object Disassemble(object value, ISessionImplementor session, object owner)
        {
            if (value != null)
            {
                return GetEnumEntityIdByEnum(value);
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) &&
                   (((PersistentEnumType) obj).ReturnedClass == ReturnedClass);
        }

        public override object FromStringValue(string xml)
        {
            return GetInstance(long.Parse(xml));
        }

        public override object Get(IDataReader rs, int index)
        {
            var code = rs[index];
            if ((code != DBNull.Value) && (code != null))
            {
                return GetInstance(code);
            }
            return null;
        }

        public override object Get(IDataReader rs, string name)
        {
            return Get(rs, rs.GetOrdinal(name));
        }

        public override SqlType SqlType
        {
            get
            {
                return _sqlType;
            }
        }

        public override int GetHashCode()
        {
            return ReturnedClass.GetHashCode();
        }

        public virtual object GetInstance(object code)
        {
            if (code.GetType() != _convertionRule.EnumEntityIdentifierType)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                code = DBNull.Value == code
                           ? null
                           : Convert.ChangeType(code, _convertionRule.EnumEntityIdentifierType);
                // ReSharper restore AssignNullToNotNullAttribute
            }
            var enumEntityRepository = new EnumEntityProvider();
            var enumValue = enumEntityRepository.GetEnumById(code, _convertionRule) ??
                            _convertionRule.GetEnumForNull();
            return enumValue;
        }

        public object GetEnumEntityIdByEnum(object code)
        {
            var provider = new EnumEntityProvider();
            object enumEntityId = provider.GetEnumEntityIdByEnum(code, _convertionRule);
            return enumEntityId;
        }

        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            return GetEnumEntityIdByEnum(value).ToString();
        }

        public override void Set(IDbCommand cmd, object value, int index)
        {
            var parameter = (IDataParameter) cmd.Parameters[index];
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                if (value is System.Enum)
                {
                    value = GetEnumEntityIdByEnum(value);
                }
                else
                {
                    if (System.Enum.GetUnderlyingType(_convertionRule.EnumType) == value.GetType())
                    {
                        value = System.Enum.ToObject(_convertionRule.EnumType, value);
                        value = GetEnumEntityIdByEnum(value);
                    }
                    else
                    {
                        value = Convert.ChangeType(value, _convertionRule.EnumEntityIdentifierType);
                    }
                }
                parameter.Value = value ?? DBNull.Value;
            }
        }

        public override string ToString(object value)
        {
            return value != null ? GetEnumEntityIdByEnum(value).ToString() : null;
        }

        public override string Name
        {
            get { return ReturnedClass.FullName; }
        }
    }
}