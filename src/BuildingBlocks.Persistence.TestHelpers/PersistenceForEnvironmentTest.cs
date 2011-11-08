using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using BuildingBlocks.Configuration.AssemblyLoad;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.Persistence.TestHelpers
{
    [TestClass]
    public abstract class PersistenceForEnvironmentTest : PersistenceTest
    {
        private static readonly ILog _logger = LogManager.GetCurrentClassLogger();

        protected override IEnumerable<Assembly> DependencyRegistriesAssemblies
        {
            get { return GetEnvironmentAssemblies(); }
        }
        
        private IEnumerable<Assembly> GetEnvironmentAssemblies()
        {
            var domainAssembliesLoader = FileSystemAssembliesLoader
                .LoaderForDllsContained("Tests.Environment", GetCurrentAssemblyPath());
            var environmentAssemblies = domainAssembliesLoader.LoadAssemblies();
            if (environmentAssemblies.Length == 0)
            {
                throw new ConfigurationErrorsException("No found test environment assemblies");
            }
            _logger.Debug(m =>
            {
                m("Found " + environmentAssemblies.Length + "-st test environment assemblies" + Environment.NewLine);
                foreach (var environmentAssembly in environmentAssemblies)
                {
                    m(environmentAssembly + Environment.NewLine);
                }
            });
            return environmentAssemblies;
        }

        private string GetCurrentAssemblyPath()
        {
            return Path.GetDirectoryName(GetType().Assembly.Location);
        }
    }
}