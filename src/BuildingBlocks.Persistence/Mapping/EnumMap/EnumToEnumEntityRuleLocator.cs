using System.Collections.Generic;
using BuildingBlocks.Common.Utils;
using StructureMap;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    internal class EnumToEnumEntityRuleLocator
    {
        private static readonly object _lockObject = new object();
        
        public static EnumToEnumEntityRuleLocator Get()
        {
            var locator = ObjectFactory.TryGetInstance<EnumToEnumEntityRuleLocator>();
            if (locator == null)
            {
                lock (_lockObject)
                {
                    locator = new EnumToEnumEntityRuleLocator();
                    ObjectFactory.Configure(conf => conf
                        .For<EnumToEnumEntityRuleLocator>()
                        .Singleton()
                        .Use(locator));
                }
            }
            return locator;
        }

        private readonly Dictionary<EnumToEnumEntityRuleKey, EnumToEnumEntityConvertionRule> _rules;

        private EnumToEnumEntityRuleLocator()
        {
            _rules = new Dictionary<EnumToEnumEntityRuleKey, EnumToEnumEntityConvertionRule>();
        }

        public void AddRule(EnumToEnumEntityRuleKey ruleKey, EnumToEnumEntityConvertionRule rule)
        {
            lock (_lockObject)
            {
                _rules.AddOrUpdate(ruleKey, rule);
            }
        }

        public EnumToEnumEntityConvertionRule GetRule(EnumToEnumEntityRuleKey key)
        {
            return _rules[key];
        }
    }
}