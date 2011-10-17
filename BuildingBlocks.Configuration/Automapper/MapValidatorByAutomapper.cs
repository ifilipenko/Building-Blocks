using System;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace BuildingBlocks.Configuration.Automapper
{
    class MapValidatorByAutomapper : IMapValidator
    {
        private readonly PropertyInfo _configurationProviderProp;

        public MapValidatorByAutomapper()
        {
            _configurationProviderProp = typeof(Mapper).GetProperty("ConfigurationProvider", BindingFlags.Static | BindingFlags.NonPublic);
        }

        public void ValidateMapForType(Type destinationType)
        {
            var provider = (ConfigurationStore)_configurationProviderProp.GetValue(null, null);
            var maps = provider.GetAllTypeMaps().Where(m => m.DestinationType == destinationType);
            foreach (var typeMap in maps)
            {
                provider.AssertConfigurationIsValid(typeMap);    
            }
        }
    }
}