using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoPoco;
using AutoPoco.Configuration;
using AutoPoco.Engine;
using Fasterflect;

namespace BuildingBlocks.Autopoco.Helpers
{
    public class DataGeneratorFactory
    {
        private readonly Lazy<IGenerationSessionFactory> _factory;
        private readonly Dictionary<Type, Action<object>> _builders;

        public DataGeneratorFactory(bool enableDefaultConventions = false)
        {
            _builders = new Dictionary<Type, Action<object>>();
            _factory = new Lazy<IGenerationSessionFactory>(() => CreateFactory(enableDefaultConventions));
        }

        public void AddBuildersFromAssemblyOf<T>()
        {
            AddBuildersFromAssembly(typeof(T).Assembly);
        }

        public void AddBuildersFromAssemblyOf(object instance)
        {
            AddBuildersFromAssembly(instance.GetType().Assembly);
        }

        public void AddBuildersFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface || !type.Implements(typeof(IPocoBuilder<>)))
                    continue;

                var pocoBuilder = Activator.CreateInstance(type);
                var builderInterface = type.GetInterface(typeof(IPocoBuilder<>).Name);
                var pocoType = builderInterface.GetGenericArguments().Single();
                _builders[pocoType] = poco => pocoBuilder.CallMethod("Setup", poco);
            }
        }

        public void AddBuilderFor<T>(Action<IEngineConfigurationTypeBuilder<T>> builderAction)
        {
            _builders[typeof(T)] = poco => builderAction((IEngineConfigurationTypeBuilder<T>)poco);
        }

        public IGenerationSession GetGenerator()
        {
            var session = _factory.Value.CreateSession();
            return session;
        }

        private IGenerationSessionFactory CreateFactory(bool enableDefaultConventions)
        {
            return AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    if (enableDefaultConventions)
                    {
                        c.UseDefaultConventions();
                    }
                });

                foreach (var builderAction in _builders)
                {
                    var builder = x.CallMethod(new[] { builderAction.Key }, "Include", Flags.InstancePublic);
                    builderAction.Value(builder);
                }
            });
        }
    }
}
