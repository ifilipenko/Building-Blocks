using System;
using BuildingBlocks.Persistence.Exceptions;
using NHibernate.SqlTypes;

namespace BuildingBlocks.Persistence.Mapping
{
    public static class SqlTypeConverter
    {
        public static SqlType GetSqlType(Type type, int stringLength = 0)
        {
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return SqlTypeFactory.Boolean;
                case TypeCode.Byte:
                    return SqlTypeFactory.Byte;
                case TypeCode.Char:
                    return SqlTypeFactory.Byte;
                case TypeCode.DateTime:
                    return SqlTypeFactory.DateTime;
                case TypeCode.Decimal:
                    return SqlTypeFactory.Decimal;
                case TypeCode.Double:
                    return SqlTypeFactory.Double;
                case TypeCode.Int16:
                    return SqlTypeFactory.Int16;
                case TypeCode.Int32:
                    return SqlTypeFactory.Int32;
                case TypeCode.Int64:
                    return SqlTypeFactory.Int64;
                case TypeCode.SByte:
                    return SqlTypeFactory.SByte;
                case TypeCode.Single:
                    return SqlTypeFactory.Single;
                case TypeCode.String:
                    return SqlTypeFactory.GetString(stringLength);
                case TypeCode.UInt16:
                    return SqlTypeFactory.UInt16;
                case TypeCode.UInt32:
                    return SqlTypeFactory.UInt32;
                case TypeCode.UInt64:
                    return SqlTypeFactory.UInt64;
                default:
                    throw new SqlTypeConvertionException(type);
            }
        }
    }
}