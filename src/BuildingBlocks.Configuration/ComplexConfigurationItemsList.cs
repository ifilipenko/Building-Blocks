using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Common;
using BuildingBlocks.Common.Reflection;
using Common.Logging;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Configuration
{
    class ComplexConfigurationItemsList : List<IConfigurationItem>, IComplexConfigurationItemsList
    {
        private static readonly ILog _log = LogManager.GetLogger<ComplexConfigurationItemsList>();
        private readonly HashSet<Type> _alreadyAddedItems;

        public ComplexConfigurationItemsList(HashSet<Type> alreadyAddedItems)
        {
            _alreadyAddedItems = alreadyAddedItems;
        }

        public void IncludeAllItemsFromAssemblies(Action<AssembliesListBuilder> assembliesSelector)
        {
            Condition.Requires(assembliesSelector, "assembliesSelector").IsNotNull();

            var assemblies = new List<Assembly>();
            var assembliesListBuilder = new AssembliesListBuilder(assemblies);
            assembliesSelector(assembliesListBuilder);

            var configurationItemTypes = assemblies
                .SelectMany(assembly =>
                            assembly.GetTypes().Where(t =>
                                                      t.IsImplementInterface<IConfigurationItem>() &&
                                                      !_alreadyAddedItems.Contains(t)))
                .ToList();

            foreach (var configurationItemType in configurationItemTypes)
            {
                if (!configurationItemType.HasParameterlessConstructor())
                {
                    _log.Debug(m => m("Item \"{0}\" has no default constructor and was ignored", configurationItemType));
                    return;
                }

                var configurationItem = (IConfigurationItem) Activator.CreateInstance(configurationItemType);
                Add(configurationItem);
                _log.Debug(m => m("Included configuration item \"{0}\"", configurationItemType));
            }
        }
    }
}