using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Configuration;

namespace BuildingBlocks.TestHelpers.DataGenerator.Rules
{
    public class GenerationRulesScanner : AssemblyInterfaceImplementationScanner
    {
        private readonly static Type _generationRulesType = typeof(IEntityGenerationRules<>);
        
        public GenerationRulesScanner(IEnumerable<Assembly> assemblies)
            : base(assemblies)
        {
        }

        public IEnumerable<EntityGenerationRuleTypeDescriptor> GenerationRuleTypes
        {
            get 
            {
                return ScanProducts
                    .Select(x => new EntityGenerationRuleTypeDescriptor(x.Type, x.Interfaces))
                    .ToList(); 
            }
        } 

        protected override bool ClassIsMatched(Type type)
        {
            return !type.IsGenericType;
        }

        protected override bool InterfaceIsMatched(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _generationRulesType;
        }
    }
}