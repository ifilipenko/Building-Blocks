using System;
using System.Collections.Generic;
using System.Globalization;

namespace BuildingBlocks.Common.Utils
{
    public class DateTimeCultureHelper
    {
        private static readonly Dictionary<string, DateTimeCultureHelper> _helpers;
        private static readonly CultureInfo _ruRUCultureInfo;
        private static readonly CultureInfo _enCultureInfo;

        static DateTimeCultureHelper()
        {
            _ruRUCultureInfo = CultureInfo.GetCultureInfo("ru-RU");
            _enCultureInfo = CultureInfo.GetCultureInfo("en");
            _helpers = new Dictionary<string, DateTimeCultureHelper>();
        }

        public static DateTimeCultureHelper Ru
        {
            get { return GetCultureHelper(_ruRUCultureInfo); }
        }

        public static DateTimeCultureHelper En
        {
            get { return GetCultureHelper(_enCultureInfo); }
        }

        public static DateTimeCultureHelper Current
        {
            get { return GetCultureHelper(CultureInfo.CurrentCulture); }
        }

        public CultureInfo Culture
        {
            get { return _culture; }
        }

        private static DateTimeCultureHelper GetCultureHelper(CultureInfo currentCulture)
        {
            if (!_helpers.ContainsKey(currentCulture.Name))
            {
                _helpers.Add(currentCulture.Name,
                             new DateTimeCultureHelper(currentCulture));
            }
            return _helpers[currentCulture.Name];
        }

        private readonly Dictionary<string, DayOfWeek> _dayOfWeekMap;
        private readonly CultureInfo _culture;

        private DateTimeCultureHelper(CultureInfo culture)
        {
            _culture = culture;
            var dateTimeFormat = culture.DateTimeFormat;

            _dayOfWeekMap = new Dictionary<string, DayOfWeek>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                var dayName = dateTimeFormat.GetDayName(dayOfWeek);
                var abbrDayName = dateTimeFormat.GetAbbreviatedDayName(dayOfWeek);
                var shortDayName = dateTimeFormat.GetShortestDayName(dayOfWeek);

                _dayOfWeekMap[dayName.ToLower()] = dayOfWeek;
                _dayOfWeekMap[abbrDayName.ToLower()] = dayOfWeek;
                _dayOfWeekMap[shortDayName.ToLower()] = dayOfWeek;
            }
        }

        public DayOfWeek GetDayOfWeek(string day)
        {
            return _dayOfWeekMap[day.ToLower()];
        }

        public string Abbreviation(DayOfWeek dayOfWeek)
        {
            return Culture.DateTimeFormat.GetAbbreviatedDayName(dayOfWeek);
        }

        public string ReadabilityDate(DateTime dateTime)
        {
            return dateTime.ToString("dd MMMM yyyy", Culture.DateTimeFormat);
        }

        public string ReadabilityTime(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm", Culture.DateTimeFormat);
        }
    }
}