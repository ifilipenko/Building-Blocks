using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace BuildingBlocks.Common
{
    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public struct MicDateTime : IComparable, IFormattable, IConvertible, ISerializable, IComparable<DateTime>, IEquatable<DateTime>
    {
        private static Func<DateTime> _nowGetter;

        static MicDateTime()
        {
            SetDefaultNowGetter();
        }

        public static void SetNowGetter(Func<DateTime> nowGetter)
        {
            _nowGetter = nowGetter;
        }

        public static void SetDefaultNowGetter()
        {
            _nowGetter = () => DateTime.Now;
        }

        private readonly DateTime _innerDateTime;
        public static readonly MicDateTime MinValue;
        public static readonly MicDateTime MaxValue;

        public MicDateTime(DateTime dateTime)
        {
            _innerDateTime = dateTime;
        }

        public MicDateTime(long ticks)
        {
            _innerDateTime = new DateTime(ticks);
        }

        public MicDateTime(long ticks, DateTimeKind kind)
        {
            _innerDateTime = new DateTime(ticks, kind);
        }

        public MicDateTime(int year, int month, int day)
        {
            _innerDateTime = new DateTime(year, month, day);
        }

        public MicDateTime(int year, int month, int day, Calendar calendar)
        {
            _innerDateTime = new DateTime(year, month, day, calendar);
        }

        public MicDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            _innerDateTime = new DateTime(year, month, day, hour, minute, second);
        }

        public MicDateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
        {
            _innerDateTime = new DateTime(year, month, day, hour, minute, second, kind);
        }

        public MicDateTime(int year, int month, int day, int hour, int minute, int second, Calendar calendar)
        {
            _innerDateTime = new DateTime(year, month, day, hour, minute, second, calendar);
        }

        public MicDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            _innerDateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        public MicDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
        {
            _innerDateTime = new DateTime(year, month, day, hour, minute, second, millisecond, kind);
        }

        public MicDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar)
        {
            _innerDateTime = new DateTime(year, month, day, hour, minute, second, millisecond, calendar);
        }

        public MicDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, DateTimeKind kind)
        {
            _innerDateTime = new DateTime(year, month, day, hour, minute, second, millisecond, calendar, kind);
        }

        public static MicDateTime operator +(MicDateTime d, TimeSpan t)
        {
            var date = d._innerDateTime + t;
            return new MicDateTime(date);
        }

        public static MicDateTime operator -(MicDateTime d, TimeSpan t)
        {
            var date = d._innerDateTime - t;
            return new MicDateTime(date);
        }

        public static TimeSpan operator -(MicDateTime d1, MicDateTime d2)
        {
            return d1._innerDateTime - d2._innerDateTime;
        }

        public static bool operator ==(MicDateTime d1, MicDateTime d2)
        {
            return d1._innerDateTime == d2._innerDateTime;
        }

        public static bool operator !=(MicDateTime d1, MicDateTime d2)
        {
            return d1._innerDateTime != d2._innerDateTime;
        }

        public static bool operator <(MicDateTime d1, MicDateTime d2)
        {
            return d1._innerDateTime < d2._innerDateTime;
        }

        public static bool operator <=(MicDateTime d1, MicDateTime d2)
        {
            return d1._innerDateTime < d2._innerDateTime;
        }

        public static bool operator >(MicDateTime d1, MicDateTime d2)
        {
            return d1._innerDateTime > d2._innerDateTime;
        }

        public static bool operator >=(MicDateTime d1, MicDateTime d2)
        {
            return d1._innerDateTime >= d2._innerDateTime;
        }

        public MicDateTime Add(TimeSpan value)
        {
            var date = _innerDateTime.Add(value);
            return new MicDateTime(date);
        }

        public MicDateTime AddDays(double value)
        {
            var date = _innerDateTime.AddDays(value);
            return new MicDateTime(date);
        }

        public MicDateTime AddHours(double value)
        {
            var date = _innerDateTime.AddHours(value);
            return new MicDateTime(date);
        }

        public MicDateTime AddMilliseconds(double value)
        {
            var date = _innerDateTime.AddMilliseconds(value);
            return new MicDateTime(date);
        }

        public MicDateTime AddMinutes(double value)
        {
            var date = _innerDateTime.AddMinutes(value);
            return new MicDateTime(date);
        }

        public MicDateTime AddMonths(int value)
        {
            var date = _innerDateTime.AddMonths(value);
            return new MicDateTime(date);
        }

        public MicDateTime AddSeconds(double value)
        {
            var date = _innerDateTime.AddSeconds(value);
            return new MicDateTime(date);
        }

        public MicDateTime AddTicks(long value)
        {
            var date = _innerDateTime.AddTicks(value);
            return new MicDateTime(date);
        }

        public MicDateTime AddYears(int value)
        {
            var date = _innerDateTime.AddYears(value);
            return new MicDateTime(date);
        }

        public static int Compare(MicDateTime t1, MicDateTime t2)
        {
            return t1._innerDateTime.CompareTo(t2._innerDateTime);
        }

        public int CompareTo(object value)
        {
            if (value is MicDateTime)
            {
                return _innerDateTime.CompareTo(((MicDateTime)value)._innerDateTime);
            }
            return _innerDateTime.CompareTo(value);
        }

        public int CompareTo(DateTime value)
        {
            return _innerDateTime.CompareTo(value);
        }

        public int CompareTo(MicDateTime value)
        {
            return _innerDateTime.CompareTo(value._innerDateTime);
        }

        public static int DaysInMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        public override bool Equals(object value)
        {
            return _innerDateTime.Equals(value);
        }

        public bool Equals(DateTime value)
        {
            return _innerDateTime.Equals(value);
        }

        public bool Equals(MicDateTime value)
        {
            return _innerDateTime.Equals(value._innerDateTime);
        }

        public static bool Equals(MicDateTime t1, MicDateTime t2)
        {
            return DateTime.Equals(t1._innerDateTime, t2._innerDateTime);
        }

        public static MicDateTime FromBinary(long dateData)
        {
            var date = DateTime.FromBinary(dateData);
            return new MicDateTime(date);
        }

        public static MicDateTime FromFileTime(long fileTime)
        {
            var date = DateTime.FromFileTime(fileTime);
            return new MicDateTime(date);
        }

        public static MicDateTime FromFileTimeUtc(long fileTime)
        {
            var date = DateTime.FromFileTimeUtc(fileTime);
            return new MicDateTime(date);
        }

        public static MicDateTime FromOADate(double d)
        {
            var date = DateTime.FromOADate(d);
            return new MicDateTime(date);
        }

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable)_innerDateTime).GetObjectData(info, context);
        }

        public bool IsDaylightSavingTime()
        {
            return _innerDateTime.IsDaylightSavingTime();
        }

        public static MicDateTime SpecifyKind(MicDateTime value, DateTimeKind kind)
        {
            var date = DateTime.SpecifyKind(value._innerDateTime, kind);
            return new MicDateTime(date);
        }

        public long ToBinary()
        {
            return _innerDateTime.ToBinary();
        }

        public override int GetHashCode()
        {
            return _innerDateTime.GetHashCode();
        }

        public static bool IsLeapYear(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        [SecuritySafeCritical]
        public static MicDateTime Parse(string s)
        {
            var date = DateTime.Parse(s);
            return new MicDateTime(date);
        }

        [SecuritySafeCritical]
        public static MicDateTime Parse(string s, IFormatProvider provider)
        {
            var date = DateTime.Parse(s, provider);
            return new MicDateTime(date);
        }

        [SecuritySafeCritical]
        public static MicDateTime Parse(string s, IFormatProvider provider, DateTimeStyles styles)
        {
            var date = DateTime.Parse(s, provider, styles);
            return new MicDateTime(date);
        }

        public static MicDateTime ParseExact(string s, string format, IFormatProvider provider)
        {
            var date = DateTime.ParseExact(s, format, provider);
            return new MicDateTime(date);
        }

        public static MicDateTime ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var date = DateTime.ParseExact(s, format, provider, style);
            return new MicDateTime(date);
        }

        public static MicDateTime ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style)
        {
            var date = DateTime.ParseExact(s, formats, provider, style);
            return new MicDateTime(date);
        }

        public TimeSpan Subtract(DateTime value)
        {
            return _innerDateTime.Subtract(value);
        }

        public MicDateTime Subtract(TimeSpan value)
        {
            var date = _innerDateTime.Subtract(value);
            return new MicDateTime(date);
        }

        public double ToOADate()
        {
            return _innerDateTime.ToOADate();
        }

        public long ToFileTime()
        {
            return _innerDateTime.ToFileTime();
        }

        public long ToFileTimeUtc()
        {
            return _innerDateTime.ToFileTimeUtc();
        }

        public MicDateTime ToLocalTime()
        {
            var date = _innerDateTime.ToLocalTime();
            return new MicDateTime(date);
        }

        [SecuritySafeCritical]
        public string ToLongDateString()
        {
            return _innerDateTime.ToLongDateString();
        }

        [SecuritySafeCritical]
        public string ToLongTimeString()
        {
            return _innerDateTime.ToLongTimeString();
        }

        [SecuritySafeCritical]
        public string ToShortDateString()
        {
            return _innerDateTime.ToShortDateString();
        }

        [SecuritySafeCritical]
        public string ToShortTimeString()
        {
            return _innerDateTime.ToShortTimeString();
        }

        [SecuritySafeCritical]
        public override string ToString()
        {
            return _innerDateTime.ToString();
        }

        [SecuritySafeCritical]
        public string ToString(string format)
        {
            return _innerDateTime.ToString(format);
        }

        [SecuritySafeCritical]
        public string ToString(IFormatProvider provider)
        {
            return _innerDateTime.ToString(provider);
        }

        [SecuritySafeCritical]
        public string ToString(string format, IFormatProvider provider)
        {
            return _innerDateTime.ToString(format, provider);
        }

        public MicDateTime ToUniversalTime()
        {
            var date = _innerDateTime.ToUniversalTime();
            return new MicDateTime(date);
        }

        [SecuritySafeCritical]
        public static bool TryParse(string s, out MicDateTime result)
        {
            DateTime date;
            if (DateTime.TryParse(s, out date))
            {
                result = new MicDateTime(date);
                return true;
            }
            result = default(MicDateTime);
            return false;
        }

        [SecuritySafeCritical]
        public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out MicDateTime result)
        {
            DateTime date;
            if (DateTime.TryParse(s, provider, styles, out date))
            {
                result = new MicDateTime(date);
                return true;
            }
            result = default(MicDateTime);
            return false;
        }

        public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style, out MicDateTime result)
        {
            DateTime date;
            if (DateTime.TryParseExact(s, format, provider, style, out date))
            {
                result = new MicDateTime(date);
                return true;
            }
            result = default(MicDateTime);
            return false;
        }

        public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style, out MicDateTime result)
        {
            DateTime date;
            if (DateTime.TryParseExact(s, formats, provider, style, out date))
            {
                result = new MicDateTime(date);
                return true;
            }
            result = default(MicDateTime);
            return false;
        }

        public string[] GetDateTimeFormats()
        {
            return _innerDateTime.GetDateTimeFormats();
        }

        public string[] GetDateTimeFormats(IFormatProvider provider)
        {
            return _innerDateTime.GetDateTimeFormats(provider);
        }

        public string[] GetDateTimeFormats(char format)
        {
            return _innerDateTime.GetDateTimeFormats(format);
        }

        public string[] GetDateTimeFormats(char format, IFormatProvider provider)
        {
            return _innerDateTime.GetDateTimeFormats(format, provider);
        }

        public TypeCode GetTypeCode()
        {
            return _innerDateTime.GetTypeCode();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return InnerIConvertible.ToBoolean(provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return InnerIConvertible.ToChar(provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return InnerIConvertible.ToSByte(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return InnerIConvertible.ToByte(provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return InnerIConvertible.ToInt16(provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return InnerIConvertible.ToUInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return InnerIConvertible.ToInt32(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return InnerIConvertible.ToUInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return InnerIConvertible.ToInt64(provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return InnerIConvertible.ToUInt64(provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return InnerIConvertible.ToSingle(provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return InnerIConvertible.ToDouble(provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return InnerIConvertible.ToDecimal(provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return InnerIConvertible.ToDateTime(provider);
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return InnerIConvertible.ToType(type, provider);
        }

        public static implicit operator DateTime(MicDateTime micDateTime)
        {
            return micDateTime._innerDateTime;
        }

        public MicDateTime Date
        {
            get
            {
                var date = _innerDateTime.Date;
                return new MicDateTime(date);
            }
        }

        public int Day
        {
            get { return _innerDateTime.Day; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return _innerDateTime.DayOfWeek; }
        }

        public int DayOfYear
        {
            get { return _innerDateTime.DayOfYear; }
        }

        public int Hour
        {
            get { return _innerDateTime.Hour; }
        }

        public DateTimeKind Kind
        {
            get { return _innerDateTime.Kind; }
        }

        public int Millisecond
        {
            get { return _innerDateTime.Millisecond; }
        }

        public int Minute
        {
            get { return _innerDateTime.Minute; }
        }

        public int Month
        {
            get { return _innerDateTime.Month; }
        }

        public static MicDateTime Now
        {
            get
            {
                var date = _nowGetter();
                return new MicDateTime(date);
            }
        }

        public static MicDateTime UtcNow
        {
            get { return Now.ToUniversalTime(); }
        }

        public int Second
        {
            get { return _innerDateTime.Second; }
        }

        public long Ticks
        {
            get { return _innerDateTime.Ticks; }
        }

        public TimeSpan TimeOfDay
        {
            get { return _innerDateTime.TimeOfDay; }
        }

        public static MicDateTime Today
        {
            get { return Now.Date; }
        }

        public int Year
        {
            get { return _innerDateTime.Year; }
        }

        private IConvertible InnerIConvertible
        {
            get { return _innerDateTime; }
        }
    }
}