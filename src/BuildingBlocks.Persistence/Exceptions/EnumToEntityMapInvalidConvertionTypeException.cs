using System;

namespace BuildingBlocks.Persistence.Exceptions
{
    public class EnumToEntityMapInvalidConvertionTypeException : EnumToEntityMapException
    {
        private readonly Type _type;
        private const string MessageFormat = "Invalid convertion type {0}";

        public EnumToEntityMapInvalidConvertionTypeException(Type type) 
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