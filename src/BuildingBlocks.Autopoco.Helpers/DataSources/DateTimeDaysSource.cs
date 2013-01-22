using System;
using AutoPoco.Engine;

namespace BuildingBlocks.Autopoco.Helpers.DataSources
{
    public class DateTimeDaysSource : DatasourceBase<DateTime>
    {
        private readonly DateTime _start;
        private int _index;

        public DateTimeDaysSource(DateTime start)
        {
            _start = start;
        }

        public override DateTime Next(IGenerationContext context)
        {
            return _start.AddDays(_index++);
        }
    }
}