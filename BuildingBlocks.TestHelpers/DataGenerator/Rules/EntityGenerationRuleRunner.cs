using System;
using System.Linq;
using AutoPoco.Configuration;

namespace BuildingBlocks.TestHelpers.DataGenerator.Rules
{
    class EntityGenerationRuleRunner 
    {
        private readonly IEngineConfigurationBuilder _configurationBuilder;

        public EntityGenerationRuleRunner(IEngineConfigurationBuilder configurationBuilder)
        {
            _configurationBuilder = configurationBuilder;
        }

        public void RunRules(EntityGenerationRuleTypeDescriptor generationRuleType)
        {
            var targetObjectTypes = generationRuleType.GetTargetObjectTypes();
            var rulesInstance = generationRuleType.CreateGenerationRuleInstance();
            var setupGenerationMethod = generationRuleType.Type.GetMethod("SetupGeneration");

            foreach (var typeBuilder in targetObjectTypes.Select(GetTypeBuilder))
            {
                setupGenerationMethod
                    .Invoke(rulesInstance, new[] { typeBuilder });
            }
        }

        private object GetTypeBuilder(Type targetObjectType)
        {
            var method = _configurationBuilder.GetType().GetMethods()
                .First(m => m.Name == "Include" && m.IsGenericMethod);
            var typeBuilder = method
                .MakeGenericMethod(targetObjectType)
                .Invoke(_configurationBuilder, new object[0]);
            return typeBuilder;
        }
    }
}