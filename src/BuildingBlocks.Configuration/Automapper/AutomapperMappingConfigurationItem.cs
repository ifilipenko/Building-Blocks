using System;
using System.Collections.Generic;
using AutoMapper;
using Common.Logging;

namespace BuildingBlocks.Configuration.Automapper
{
    public class AutomapperMappingConfigurationItem : IConfigurationItem
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly AutomapperMappingParameters _parameters;
        private readonly IMapValidator _mapsValidator;

        public AutomapperMappingConfigurationItem(AutomapperMappingParameters parameters)
        {
            _parameters = parameters;
            _mapsValidator = _parameters.ValidateMaps
                                 ? (IMapValidator) new MapValidatorByAutomapper()
                                 : new NullMapValidator();
        }

        public void Configure(IocContainer iocContainer)
        {
            _logger.Debug(m => m("start automapper configuration"));

            var automapperMaps = FindAutomapperMaps();
            var targetObjectTypes = new List<Type>();
            foreach (var mapType in automapperMaps)
            {
                var map = Activator.CreateInstance(mapType.Type);
                foreach (var targetObjectType in mapType.GetTargetObjectTypes())
                {
                    var factoryType = typeof (AutomapperMapFactory<>).MakeGenericType(targetObjectType);
                    var factoryIfaceType = typeof (IAutomapperMapFactory<>).MakeGenericType(targetObjectType);
                    var factory = Activator.CreateInstance(factoryType);

                    _logger.Debug(m => m("Invoke map {0}", mapType.Type));
                    var method = mapType.Type.GetMethod("CreateMaps", new[] { factoryIfaceType });
                    method.Invoke(map, new[] { factory });

                    targetObjectTypes.Add(targetObjectType);
                    //_mapsValidator.ValidateMapForType(targetObjectType);
                }
            }
            foreach (var targetObjectType in targetObjectTypes)
            {
                _mapsValidator.ValidateMapForType(targetObjectType);
            }

            Mapper.AllowNullDestinationValues = true;
            _logger.Debug(m => m("end automapper configuration"));
        }

        private IEnumerable<InterfacesImplemenation> FindAutomapperMaps()
        {
            _logger.Debug(m => 
            {
                if (_parameters.MappingAssemblies.Length == 0)
                {
                    m("Automapper map scan assembles list is empty");
                }
                else
                {
                    foreach (var mappingAssembly in _parameters.MappingAssemblies)
                    {
                        m("Search automapper maps from assembly {0}{1}", mappingAssembly, Environment.NewLine);
                    }
                }
            });

            var scanner = new FindAutomapperMaps(_parameters.MappingAssemblies);
            scanner.Scan();
            var result = scanner.FoundedAutomapperMaps;

            _logger.Debug(m => 
            {
                foreach (var type in result)
                {
                    m("Found automapper maps {0}{1}", type.Type, Environment.NewLine);
                }
            });

            return result;
        }
    }
}