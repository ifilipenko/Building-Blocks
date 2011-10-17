using System;
using BuildingBlocks.Common.Translate;

namespace BuildingBlocks.Common.ValueObjects
{
    public class Age
    {
        public Age(DateTime date, DateTime currentDate)
        {
            SetParams(date, currentDate);
        }

        public int Years { get; private set; }
        public int Months { get; private set; }
        public int Days { get; private set; }
        public string SpecificAge { get; private set; }

        private void SetParams(DateTime value, DateTime currentDate)
        {
            var dYears = currentDate.Year - value.Year;
            var dMonths = currentDate.Month - value.Month;
            var dDays = currentDate.Day - value.Day;

            var calcYears = dYears;
            if (dMonths < 0 ||
                (dMonths == 0 && dDays < 0))
                calcYears--;

            var calcMonths = 12 * dYears + dMonths;
            if (dDays < 0)
                calcMonths--;

            var calcDays = (currentDate.Date - value.Date).Days;

            var ageText = BuildSpecificAge(calcYears, calcMonths, calcDays);

            Years = calcYears;
            Months = calcMonths;
            Days = calcDays;
            SpecificAge = ageText;
        }

        private static string BuildSpecificAge(int years, int months, int days)
        {
            int ageValue;
            RusNumber.GrammaticalNumbers grammaticalNumbers;
            if (years > 0)
            {
                ageValue = years;
                grammaticalNumbers = RusNumber.Years;
            }
            else if (months > 0)
            {
                ageValue = months;
                grammaticalNumbers = RusNumber.Months;
            }
            else
            {
                ageValue = days;
                grammaticalNumbers = RusNumber.Days;
            }
            return string.Format("{0} {1}", ageValue, ageValue.GrammaticalNumberForValue(grammaticalNumbers));
        }
    }
}