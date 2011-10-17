using System;

namespace BuildingBlocks.Common.ValueObjects
{
    public struct DateRange : IEquatable<DateRange>
    {
        private Date _start;
        private Date _stop;

        public DateRange(Date start, Date stop)
        {
            _start = start;
            _stop = stop;
            SetRange(start, stop);
        }

        public Date Start
        {
            get { return _start; }
        }

        public Date Stop
        {
            get { return _stop; }
        }

        public bool IsPoint
        {
            get { return _start == _stop; }
        }

        public bool IsBoundary
        {
            get { return _start.IsBoundary && _stop.IsBoundary; }
        }

        public void SetRange(Date start, Date stop)
        {
            _start = start;
            _stop = stop;
        }

        public int LenghtInDays
        {
            get
            {
                Validate();
                TimeSpan timeSpan = (DateTime)Start - (DateTime)Stop;
                return timeSpan.Days + 1;
            }
        }

        public bool IsIncludeRange(DateRange range)
        {
            Validate();
            range.Validate();
            return Start.CompareTo(range.Start) >= 0 && Stop.CompareTo(range.Stop) <= 0;
        }

        public bool AreNotCrossed(DateRange range)
        {
            Validate();
            range.Validate();
            return (Start.CompareTo(range.Stop) > 0) || (Stop.CompareTo(range.Start) < 0);
        }

        public override string ToString()
        {
            Validate();
            return string.Format("{0} [{1} - {2}]} ", typeof(DateRange).Name, Start, Stop);
        }

        public override bool Equals(object obj)
        {
            Validate();
            if (!(obj is DateRange)) return false;
            return Equals((DateRange)obj);
        }

        public bool Equals(DateRange obj)
        {
            Validate();
            return obj._start.Equals(_start) && obj._stop.Equals(_stop);
        }

        public override int GetHashCode()
        {
            Validate();
            unchecked
            {
                return (_start.GetHashCode() * 397) ^ _stop.GetHashCode();
            }
        }

        public static bool operator ==(DateRange left, DateRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DateRange left, DateRange right)
        {
            return !left.Equals(right);
        }

        #region [Validation]

        private void Validate()
        {
            if (!IsValid)
            {
                throw new InvalidOperationException("Interval validation failed with message: " + GetValidationMessage());
            }
        }

        public bool IsValid
        {
            get { return string.IsNullOrEmpty(GetValidationMessage()); }
        }

        public string GetValidationMessage()
        {
            return _start.CompareTo(_stop) > 0 ? "Начало интервала должно быть больше его окончания" : string.Empty;
        }

        #endregion

    }
}