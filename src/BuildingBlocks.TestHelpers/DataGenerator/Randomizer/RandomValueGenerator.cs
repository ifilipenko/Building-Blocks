using System;
using System.Text;
using BuildingBlocks.Common;
using BuildingBlocks.Common.Reflection;

namespace BuildingBlocks.TestHelpers.DataGenerator.Randomizer
{
    class RandomValueGenerator
    {
        static readonly Random _random = new Random();

        public T GenerateValue<T>(int sizeIfTIsString)
        {
            return (T) GenerateValue(typeof(T), sizeIfTIsString);
        }

        public object GenerateValue(Type type, int sizeIfString)
        {
            if (type.IsEnum)
            {
                return GetRandomEnumValue(type);
            }

            if (type == typeof(MicDateTime))
            {
                return new MicDateTime(GenerateDateTime());
            }

            TypeCode typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return (_random.NextDouble() > 0.5);
                case TypeCode.Char:
                    return (Convert.ToChar(_random.Next(Byte.MinValue, Byte.MaxValue)));
                case TypeCode.SByte:
                    return (Convert.ToSByte(_random.Next(SByte.MinValue, SByte.MaxValue)));
                case TypeCode.Byte:
                    return (Convert.ToByte(_random.Next(Byte.MinValue, Byte.MaxValue)));
                case TypeCode.Int16:
                    return (Convert.ToInt16(_random.Next(Int16.MinValue, Int16.MaxValue)));
                case TypeCode.UInt16:
                    return (Convert.ToUInt16(_random.Next(UInt16.MinValue, UInt16.MaxValue)));
                case TypeCode.Int32:
                    return (Convert.ToInt32(_random.Next(Int32.MinValue, Int32.MaxValue)));
                case TypeCode.UInt32:
                    return (Convert.ToUInt32(_random.Next((int)UInt32.MinValue, Int32.MaxValue)));
                case TypeCode.Int64:
                    return (Convert.ToInt64(_random.Next(Int32.MinValue, Int32.MaxValue)));
                case TypeCode.UInt64:
                    return Convert.ToUInt64(_random.Next(Int32.MinValue, Int32.MaxValue));
                case TypeCode.Single:
                    return Convert.ToSingle(RandomDecimal());
                case TypeCode.Double:
                    return Convert.ToDouble(RandomDecimal());
                case TypeCode.Decimal:
                    return Convert.ToDecimal(RandomDecimal());
                case TypeCode.DateTime:
                    return GenerateDateTime();
                case TypeCode.String:
                    return GetRandomStringValue(sizeIfString);
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    if (type.IsNullableType())
                    {
                        Type nullableType = type.GetNonNullableType();
                        if (type != nullableType)
                            return GenerateValue(nullableType, sizeIfString);
                    }
                    throw new NotSupportedException("Generate random value for type " + type.Name + " not supported");
            }

            return null;
        }

        private static DateTime GenerateDateTime()
        {
            int days = _random.Next(0, 30);
            int second = _random.Next(0, 3600);
            return DateTime.Now.AddDays(days).AddSeconds(second);
        }

        private static decimal RandomDecimal()
        {
            const decimal minValue = -10000;
            const decimal maxValue = 20000;
            const decimal intLength = maxValue - minValue;

            decimal randomValue = (decimal)_random.NextDouble();
            return randomValue * intLength + minValue;
        }

        private static object GetRandomEnumValue(Type type)
        {
            Array values = Enum.GetValues(type);
            if (values.Length == 0)
                throw new InvalidOperationException("Enum has no values");

            int valueIndex = _random.Next(0, values.Length);
            return values.GetValue(valueIndex);
        }

        private static string GetRandomStringValue(int size)
        {
            var result = new StringBuilder();
            while (result.Length < size)
            {
                int minLetterCode = Math.Min((byte)'A', (byte)'a');
                int maxLetterCode = Math.Max((byte)'Z', (byte)'z') + 1;

                char randChar;
                do
                {
                    randChar = (char)_random.Next(minLetterCode, maxLetterCode);
                } while (!char.IsLetter(randChar));

                result.Append(randChar);
            }

            if (result.Length > size)
                result.Length = size;
            return result.ToString();
        }
    }
}