using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common
{
    public class AssembliesListBuilder
    {
        private readonly ICollection<Assembly> _assemblies;

        public AssembliesListBuilder(ICollection<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public AssembliesListBuilder AddAssemblyOfType<T>()
        {
            _assemblies.Add(typeof (T).Assembly);
            return this;
        }

        public AssembliesListBuilder AddAssemblyOfType(Type type)
        {
            Condition.Requires(type, "type").IsNotNull();
            _assemblies.Add(type.Assembly);
            return this;
        }

        public AssembliesListBuilder AddAssembly(Assembly assembly)
        {
            Condition.Requires(assembly, "assembly").IsNotNull();
            _assemblies.Add(assembly);
            return this;
        }

        public AssembliesListBuilder AddAssembly(string assemblyName)
        {
            Condition.Requires(assemblyName, "assemblyName").IsNotNullOrEmpty();

            var assembly = AppDomain.CurrentDomain.Load(assemblyName);
            return AddAssembly(assembly);
        }

        public AssembliesListBuilder AddAssemblyFromAppSettings(string appSettingsKey)
        {
            var assemblyName = ConfigurationManager.AppSettings[appSettingsKey];
            if (string.IsNullOrEmpty(assemblyName))
            {
                var message = string.Format("Application settings with key \"{0}\" has invalid assembly name \"{1}\"", appSettingsKey, assemblyName);
                throw new InvalidOperationException(message);
            }
            return AddAssembly(assemblyName);
        }
    }
}