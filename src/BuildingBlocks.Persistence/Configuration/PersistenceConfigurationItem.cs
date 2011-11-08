using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Configuration;
using BuildingBlocks.Persistence.Mapping.EnumMap;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Logging.CommonLogging;
using NHibernate.Tool.hbm2ddl;
using StructureMap;

namespace BuildingBlocks.Persistence.Configuration
{
    public abstract class PersistenceConfigurationItem<TParam> : IConfigurationItem
        where TParam : PersistenceConfigurationParameters<TParam>, new()
    {
        private readonly TParam _configurationParameters;

        public PersistenceConfigurationItem(TParam configurationParameters)
        {
            _configurationParameters = configurationParameters.Clone();
        }

        protected TParam ConfigurationParameters 
        {
            get { return _configurationParameters; }
        }

        public void Configure(IocContainer iocContainer)
        {
            var configuration = Fluently.Configure();
            if (configuration == null)
                return;
            //turn on for testing
            //var logListener = new LogDiagnosticListener();
            //configuration.Diagnostics(d => d.Enable().RegisterListener(logListener));

            ConfigureConventions(iocContainer);

            configuration.ProxyFactoryFactory<ProxyFactoryFactory>();
            configuration.Database(SetupDatabaseConnection);

            configuration.Mappings(x =>
            {
                foreach (var assembly in _configurationParameters.MappingAssemblies)
                {
                    x.FluentMappings.AddFromAssembly(assembly);
                    x.HbmMappings.AddFromAssembly(assembly);
                }

                if (_configurationParameters.MappingExporter != null)
                {
                    _configurationParameters.MappingExporter.Export(x.FluentMappings);
                    _configurationParameters.MappingExporter.Export(x.AutoMappings);
                }
            });
            var nhConfiguration = configuration.BuildConfiguration();
            SetupInterceptor(nhConfiguration);
            SetupLogger(nhConfiguration);
            EnableGenerateStatistics(nhConfiguration);
            ConfigureCacheProvider(nhConfiguration);
            
            var sessionFactory = nhConfiguration.BuildSessionFactory();
            BuildSchema(nhConfiguration, sessionFactory);
            PersistenceEnvironment.InitializeFactory(sessionFactory);
            SetEnumMaps(_configurationParameters.MappingAssemblies);
        }

        private void ConfigureConventions(IocContainer iocContainer)
        {
            ObjectFactory.Configure(x => _configurationParameters.Conventions.Configure(new IocContainer(x)));
        }

        private void SetEnumMaps(IEnumerable<Assembly> mappingAssemblies)
        {
            var enumMaps = FindEnumMaps(mappingAssemblies);
            ObjectFactory.Configure(conf => conf
                .For<EnumMapContainer>()
                .Singleton()
                .Use(new EnumMapContainer(enumMaps)));
        }

        private IEnumerable<Type> FindEnumMaps(IEnumerable<Assembly> mappingAssemblies)
        {
            return mappingAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsPublic && t.IsClass)
                .Where(t => t.BaseType != null &&
                            t.BaseType.IsGenericType &&
                            t.BaseType.GetGenericTypeDefinition() == typeof (EnumMap<,>))
                .ToList();
        }

        protected abstract IPersistenceConfigurer SetupDatabaseConnection();

        private void ConfigureCacheProvider(NHibernate.Cfg.Configuration nhConfiguration)
        {
            if (_configurationParameters.CacheProviderClass == null)
            {
                nhConfiguration.SetProperty("cache.use_second_level_cache", "false");
                nhConfiguration.SetProperty("cache.use_query_cache", "false");
            }
            else
            {
                nhConfiguration.SetProperty("cache.provider_class", _configurationParameters.CacheProviderClass.AssemblyQualifiedName);
                nhConfiguration.SetProperty("cache.use_second_level_cache", "true");
                nhConfiguration.SetProperty("cache.use_query_cache", "true");
            }
        }

        private void SetupInterceptor(NHibernate.Cfg.Configuration nhConfiguration)
        {
            nhConfiguration.SetInterceptor(new EntityInterceptor());
        }

        private void EnableGenerateStatistics(NHibernate.Cfg.Configuration nhConfiguration)
        {
            nhConfiguration.SetProperty("generate_statistics", 
                _configurationParameters.EnableStatistics.ToString().ToLower());
        }

        private void BuildSchema(NHibernate.Cfg.Configuration nhConfiguration, ISessionFactory sessionFactory)
        {
            if (_configurationParameters.SchemaUpdateMode == SchemaUpdateMode.Disabled)
                return;

            using (var session = sessionFactory.OpenSession())
            {
                switch (_configurationParameters.SchemaUpdateMode)
                {
                    case SchemaUpdateMode.Update:
                        var schemaUpdate = new SchemaUpdate(nhConfiguration);
                        schemaUpdate.Execute(_configurationParameters.SchemaOutput, true);
                        break;
                    case SchemaUpdateMode.Recreate:
                        var schemaExport = new SchemaExport(nhConfiguration);
                        schemaExport.Execute(_configurationParameters.SchemaOutput, true, false, session.Connection, null);
                        break;
                }
            }
        }

        private void SetupLogger(NHibernate.Cfg.Configuration nhConfiguration)
        {
            if (_configurationParameters.EnableQueryLogging)
            {
                nhConfiguration.Properties[NHibernate.Cfg.Environment.ShowSql] = true.ToString();
                nhConfiguration.Properties[NHibernate.Cfg.Environment.FormatSql] = true.ToString();
            }

            nhConfiguration.Properties["nhibernate-logger"] = typeof (CommonLoggingLoggerFactory).AssemblyQualifiedName;
        }
    }
}