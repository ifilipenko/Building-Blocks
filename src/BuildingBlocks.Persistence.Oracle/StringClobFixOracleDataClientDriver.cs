using System;
using System.Data;
using System.Linq;
using NHibernate.Driver;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace BuildingBlocks.Persistence.Oracle
{
    /// <summary>
    /// Драйвер необходим для корректной работы с nclob полями (иначе Oracle.DataAccess работает с ними как с varchar)
    /// </summary>
    /// <remarks>
    /// Тип поля в маппинге надо указывать как CustomType(StringClob)
    /// </remarks>
    public class StringClobFixOracleDataClientDriver : OracleDataClientDriver
    {
        private readonly Type _paramType;
        private readonly Type _dbType;
        private readonly Array _dbTypeValues;

        public StringClobFixOracleDataClientDriver()
        {
            _dbType = ReflectHelper.TypeFromAssembly("Oracle.DataAccess.Client.OracleDbType",
                                                     "Oracle.DataAccess",
                                                     true);
            _paramType = ReflectHelper.TypeFromAssembly("Oracle.DataAccess.Client.OracleParameter",
                                                        "Oracle.DataAccess",
                                                        true);
            _dbTypeValues = System.Enum.GetValues(_dbType);
        }

        protected override void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
        {
            base.InitializeParameter(dbParam, name, sqlType);

            if (sqlType is StringClobSqlType)
            {
                var propertyInfo = _paramType.GetProperty("OracleDbType");
                var nclobValue = _dbTypeValues.OfType<object>().First(v => v.ToString() == "NClob");
                propertyInfo.SetValue(dbParam, nclobValue, null);
            }
        }
    }
}