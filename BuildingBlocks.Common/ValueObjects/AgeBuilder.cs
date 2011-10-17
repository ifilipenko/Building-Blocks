using System;

namespace BuildingBlocks.Common.ValueObjects
{
    public static class AgeBuilder
    {
        public static Age AgeForDate(this DateTime dateTime, DateTime currentDateTime)
        {
            return new Age(dateTime, currentDateTime);
        }

        public static Age AgeForDate(this MicDateTime dateTime, DateTime currentDateTime)
        {
            return new Age(dateTime, currentDateTime);
        }
    }
}