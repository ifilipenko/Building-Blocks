using System;
using BuildingBlocks.Common.Utils;

namespace BuildingBlocks.Common.Sugar
{
    public static class DateTimeExtensions
    {
        public static DateTimeSequenceGenerator Next(this DateTime from, int count)
        {
            return new DateTimeSequenceGenerator(from, count);
        }

        public static DateTime MinValueIfNull(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value : DateTime.MinValue;
        }

        public static DateTime MaxValueIfNull(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value : DateTime.MaxValue;
        }

        public static int GetAge(this DateTime startDate)
        {
            return GetDiffInYears(DateTime.Now, startDate);
        }

        public static int GetDiffInYears(this DateTime startDate, DateTime endDate)
        {
            if (startDate < endDate)
            {
                DateTime oldDateTime = startDate;
                startDate = endDate;
                endDate = oldDateTime;
            }

            int diffInYears = startDate.Year - endDate.Year;
            if (startDate.Month < endDate.Month || (startDate.Month == endDate.Month && startDate.Day < endDate.Day))
            {
                diffInYears--;
            }
            return diffInYears;
        }

        public static DateTime Max(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1.CompareTo(dateTime2) > 0 ? dateTime1 : dateTime2;
        }

        public static DateTime Min(this DateTime dateTime1, DateTime dateTime2)
        {
            return dateTime1.CompareTo(dateTime2) < 0 ? dateTime1 : dateTime2;
        }

        public static string ShortName(this DayOfWeek dayOfWeek)
        {
            return DateTimeCultureHelper.Current.Abbreviation(dayOfWeek);
        }
    }
}