using System;

namespace BuildingBlocks.Persistence.Exceptions
{
    public class SqlTypeConvertionException : PersistenceException
    {
        private readonly Type _type;
        private const string MessageFormat = "Invalid convertion type {0}";

        public SqlTypeConvertionException(Type type) 
        {
            _type = type;
        }

        public Type ConvertionType
        {
            get { return _type; }
        }

        public override string Message
        {
            get { return string.Format(MessageFormat, _type); }
        }
    }
}