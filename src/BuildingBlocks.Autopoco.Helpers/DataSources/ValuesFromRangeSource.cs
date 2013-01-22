using System;
using AutoPoco.Engine;

namespace BuildingBlocks.Autopoco.Helpers.DataSources
{
    public class ValuesFromRangeSource<T> : DatasourceBase<T>
    {
        private readonly T[] _range;
        private readonly Random _random = new Random(1337);

        public ValuesFromRangeSource(params T[] range)
        {
            _range = range;
        }

        public override T Next(IGenerationContext context)
        {
            var index = _random.Next(0, _range.Length);
            return _range[index];
        }
    }
}