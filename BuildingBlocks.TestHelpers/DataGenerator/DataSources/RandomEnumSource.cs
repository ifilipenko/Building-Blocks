using System;
using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.Randomizer;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class RandomEnumSource : IDatasource
    {
        private static readonly RandomValues _randomValues = new RandomValues();

        public object Next(IGenerationContext context)
        {
            Type enumType;
            if (context.Node.ContextType.HasFlag(GenerationTargetTypes.Field))
            {
                var fieldGenerationNode = (TypeFieldGenerationContextNode) context.Node;
                enumType = fieldGenerationNode.Field.FieldInfo.FieldType;
            }
            else if (context.Node.ContextType.HasFlag(GenerationTargetTypes.Property))
            {
                var propertyGenerationNode = (TypePropertyGenerationContextNode)context.Node;
                enumType = propertyGenerationNode.Property.PropertyInfo.PropertyType;
            }
            else if (context.Node.ContextType.HasFlag(GenerationTargetTypes.Method))
            {
                var methodGenerationNode = (TypeMethodGenerationContextNode)context.Node;
                enumType = methodGenerationNode.Method.MethodInfo.ReturnType;
            }
            else 
            {
                throw new NotSupportedException("This generation node is not supported!");
            }

            if (enumType == null || !enumType.IsEnum)
            {
                throw new InvalidOperationException("This member is not enum");
            }
            return _randomValues.Random(enumType);
        }
    }
}