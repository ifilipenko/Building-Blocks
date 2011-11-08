using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoPoco;
using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.Exceptions;
using BuildingBlocks.TestHelpers.DataGenerator.InstancesModifier;
using BuildingBlocks.TestHelpers.DataGenerator.Rules;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public class DataGenerator
    {
        protected readonly IGenerationSessionFactory _factory;
        private IGenerationSession _session;
        private readonly IEnumerable<EntityGenerationRuleTypeDescriptor> _ruleTypes;

        protected DataGenerator()
        {
        }

        public DataGenerator(IEnumerable<Assembly> generationRulesAssemblies)
        {
            var rulesScanner = new GenerationRulesScanner(generationRulesAssemblies);
            rulesScanner.Scan();
            var ruleTypes = rulesScanner.GenerationRuleTypes;
            
            _factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.ScanAssemblyWithType<DataGenerator>();
                    foreach (var assembly in generationRulesAssemblies)
                    {
                        c.ScanAssembly(assembly);
                    }
                });
                var ruleRunner = new EntityGenerationRuleRunner(x);
                
                foreach (var ruleType in ruleTypes)
                {
                    ruleRunner.RunRules(ruleType);
                }
            });

            _ruleTypes = ruleTypes;
        }

        private IGenerationSession Session
        {
            get { return _session ?? (_session = _factory.CreateSession()); }
        }

        public IObjectGenerator<TPoco> Single<TPoco>()
        {
            try
            {
                return Session.Single<TPoco>();
            }
            catch (ArgumentException)
            {
                if (_ruleTypes.Any(r => r.Type == typeof(TPoco)))
                {
                    throw;
                }
                throw new DataGeneratorException("Rule for type " + typeof (TPoco) + " is not exists");
            }
        }

        public ICollectionContext<TPoco, IList<TPoco>> List<TPoco>(int count)
        {
            return Session.List<TPoco>(count);
        }

        public IInstancesModifier<T> For<T>(IEnumerable<T> enumerable)
        {
            return new InstancesModifier<T>(enumerable);
        }

        public IListBuilder<T> CreateList<T>()
        {
            return new ListBuilder<T>();
        }
    }
}