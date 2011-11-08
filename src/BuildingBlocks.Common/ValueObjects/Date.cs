using System;

namespace BuildingBlocks.Common.ValueObjects
{
    public struct Date : IComparable, IComparable<DateTime>, IComparable<Date>, IEquatable<Date>
    {
        public readonly static Date MinValue = new Date(DateTime.MinValue);
        public readonly static Date MaxValue = new Date(DateTime.MaxValue);
        public static Date Now
        {
            get { return new Date(DateTime.Now); }
        }

        private DateTime? _innerDateTime;

        public Date(DateTime innerDateTime)
        {
            _innerDateTime = innerDateTime.Date;
        }

        public Date(DateTime? innerDateTime)
        {
            _innerDateTime = innerDateTime.HasValue ? innerDateTime.Value.Date : new DateTime().Date;
        }

        DateTime InnerDateTime
        {
            get
            {
                if (!_innerDateTime.HasValue)
                {
                    _innerDateTime = DateTime.MinValue;
                }
                return _innerDateTime.Value;
            }
        }

        public int Year
        {
            get { return InnerDateTime.Year; }
        }

        public int Month
        {
            get { return InnerDateTime.Month; }
        }

        public int Day
        {
            get { return InnerDateTime.Day; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return InnerDateTime.DayOfWeek; }
        }

        public int DayOfYear
        {
            get { return InnerDateTime.DayOfYear; }
        }

        public Date AddDays(double value)
        {
            return new Date(InnerDateTime.AddDays(value));
        }

        public Date AddMonths(int months)
        {
            return new Date(InnerDateTime.AddMonths(months));
        }

        public Date AddYears(int value)
        {
            return new Date(InnerDateTime.AddYears(value));
        }

        public static explicit operator DateTime(Date date)
        {
            return date.InnerDateTime;
        }

        public bool IsBoundary
        {
            get { return Equals(MinValue) || Equals(MaxValue); }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is DateTime)
            {
                return InnerDateTime.CompareTo(((DateTime)obj).Date);
            }
            if (obj is Date)
            {
                return InnerDateTime.CompareTo(((Date)obj).InnerDateTime);
            }
            throw new ArgumentException("object is not a DateTime or Date");
        }

        #endregion

        #region IComparable<DateTime> Members

        public int CompareTo(DateTime other)
        {
            return InnerDateTime.CompareTo(other.Date);
        }

        #endregion

        #region IComparable<Date> Members

        public int CompareTo(Date other)
        {
            return InnerDateTime.CompareTo(other.InnerDateTime);
        }

        #endregion


        public string ToString(string format)
        {
            return InnerDateTime.ToString(format);
        }

        public override string ToString()
        {
            return InnerDateTime.ToShortDateString();
        }

        public override bool Equals(object obj)
        {
            return (obj is Date) && Equals((Date)obj);
        }

        public bool Equals(Date obj)
        {
            return obj.InnerDateTime.Equals(InnerDateTime);
        }

        public override int GetHashCode()
        {
            return InnerDateTime.GetHashCode();
        }

        public static bool operator ==(Date left, Date right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Date left, Date right)
        {
            return !left.Equals(right);
        }
    }
}