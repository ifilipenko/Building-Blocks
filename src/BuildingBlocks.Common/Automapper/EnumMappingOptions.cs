using System;
using System.Collections.Generic;

namespace BuildingBlocks.Common.Automapper
{
    public class EnumMappingOptions<TResult> : IEnumMappingSetup<TResult>
    {
        private readonly Dictionary<object, TResult> _fromSourceToDestinationValuesMap;
        private readonly List<object> _ignoreEnumList;

        public EnumMappingOptions()
        {
            _ignoreEnumList = new List<object>(0);
            _fromSourceToDestinationValuesMap = new Dictionary<object, TResult>(0);
        }

        public void Ignore<T>(T sourceEnumValue) 
            where T : struct
        {
            _ignoreEnumList.Add(sourceEnumValue);
        }

        public IEnumValueMappingSetup<TResult> Source<T>(T sourceEnumValue) 
            where T : struct
        {
            return new EnumValueMappingSetup(this, sourceEnumValue);
        }

        public bool ShouldBeIgnored(object sourceEnum)
        {
            return _ignoreEnumList.Contains(sourceEnum);
        }

        public bool TryMapByRules(object sourceEnum, out TResult result)
        {
            return _fromSourceToDestinationValuesMap.TryGetValue(sourceEnum, out result);
        }

        private class EnumValueMappingSetup : IEnumValueMappingSetup<TResult>
        {
            private readonly EnumMappingOptions<TResult> _enumMappingOptions;
            private readonly ValueType _sourceEnumValue;

            public EnumValueMappingSetup(EnumMappingOptions<TResult> enumMappingOptions, ValueType sourceEnumValue)
            {
                _enumMappingOptions = enumMappingOptions;
                _sourceEnumValue = sourceEnumValue;
            }

            public IEnumMappingSetup<TResult> MapTo(TResult destinationEnumValue)
            {
                _enumMappingOptions._fromSourceToDestinationValuesMap[_sourceEnumValue] = destinationEnumValue;
                return _enumMappingOptions;
            }
        }
    }
}