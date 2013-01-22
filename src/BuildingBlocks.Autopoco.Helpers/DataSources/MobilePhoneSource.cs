using System;
using AutoPoco.Engine;

namespace BuildingBlocks.Autopoco.Helpers.DataSources
{
    public class MobilePhoneSource : DatasourceBase<string>
    {
        private const long MaxDigits = 9999999999;
        private readonly Random _random = new Random(1337);
        private readonly string _prefix;
        private const int _index = 0;
        private readonly long _initialDigits;

        public MobilePhoneSource(string prefix = "+7")
        {
            _initialDigits = (long)(_random.NextDouble() * MaxDigits);
            _prefix = prefix;
        }

        public override string Next(IGenerationContext context)
        {
            var digits = (_initialDigits + _index) % MaxDigits;
            return _prefix + (digits + _index).ToString("D10");
        }
    }
}