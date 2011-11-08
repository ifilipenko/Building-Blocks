using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Common.Reflection;
using BuildingBlocks.Common.Sugar;
using Common.Logging;
using CuttingEdge.Conditions;
using StructureMap;

namespace BuildingBlocks.Configuration
{
    public class Configurator
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly List<IConfigurationItem> _items;
        private bool _isConfigured;

        public Configurator()
        {
            _items = new List<IConfigurationItem>();
        }

        public bool IsConfigured
        {
            get { return _isConfigured; }
        }

        public void ScanItemsInAssemblies(IEnumerable<Assembly> assemblies)
        {
            Condition.Requires(assemblies, "assemblies").IsNotNull().IsNotEmpty();

            try
            {
                var itemTypesToSkip = new HashSet<Type>(_items.Select(i => i.GetType()));
                var publicTypes = assemblies
                    .SelectMany(a => a.GetTypes().Where(type =>
                                                        type.IsPublic && !type.IsAbstract && type.IsClass &&
                                                        !itemTypesToSkip.Contains(type)));

                var configurationItemTypes = publicTypes
                    .Where(t => t.IsImplementInterface<IConfigurationItem>())
                    .ToList();

                foreach (var itemType in configurationItemTypes)
                {
                    var defaultCtor = itemType.GetConstructor(Type.EmptyTypes);
                    if (defaultCtor == null)
                    {
                        _logger.Debug(
                            m => m("Item type [" + itemType + "] will be skipped because it has no default constructor"));
                        continue;
                    }
                    var item = defaultCtor.Invoke(null);
                    _items.Add((IConfigurationItem) item);

                    _logger.Debug(m => m("Find and successfully add configuration item \"{0}\"", item.GetType()));
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                _logger.Error("Types loading failed", ex);
                for (int index = 0; index < ex.LoaderExceptions.Length; index++)
                {
                    var exception = (TypeLoadException) ex.LoaderExceptions[index];
                    _logger.Error(m => m("Loader exception #{0}: {1}", (index + 1), exception.TypeName));
                }
                throw new InvalidOperationException("Types scanner failed!"); 
            }
        }

        public void AddItem(IConfigurationItem configurationItem)
        {
            Condition.Requires(configurationItem, "configurationItem").IsNotNull();

            _items.Add(configurationItem);
        }

        public void Configure()
        {
            ObjectFactory.Configure(Configure);
        }

        private void Configure(ConfigurationExpression globalContainerConfiguration)
        {
            var iocContainer = new IocContainer(globalContainerConfiguration);

            var alreadyAddedItems = new HashSet<Type>(_items.Select(i => i.GetType()));
            foreach (var item in _items)
            {
                if (item is IComplexConfigurationItem)
                {
                    _logger.Debug(m => m("Start including configuration items for \"{0}\"", item.GetType()));
                    var includedItemsList = new ComplexConfigurationItemsList(alreadyAddedItems);
                    item.CastTo<IComplexConfigurationItem>().IncludeItems(includedItemsList);
                    _logger.Debug(m => m("Included {0} items with item \"{1}\"", includedItemsList.Count, item.GetType()));

                    foreach (var includedItem in includedItemsList)
                    {
                        alreadyAddedItems.Add(includedItem.GetType());
                        includedItem.Configure(iocContainer);
                    }
                }

                item.Configure(iocContainer);
            }

            _isConfigured = true;
        }
    }
}