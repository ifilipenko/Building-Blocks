using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.TestHelpers.DataGenerator.Rules
{
    public class EntityGenerationRuleTypeDescriptor 
    {
        private readonly Type _type;
        private readonly IEnumerable<Type> _interfaces;

        public EntityGenerationRuleTypeDescriptor(Type type, IEnumerable<Type> interfaces)
        {
            _type = type;
            _interfaces = interfaces;
        }

        public IEnumerable<Type> Interfaces
        {
            get { return _interfaces; }
        }

        public Type Type
        {
            get { return _type; }
        }

        public IEnumerable<Type> GetTargetObjectTypes()
        {
            return _interfaces.SelectMany(s => s.GetGenericArguments()).ToArray();
        }

        public object CreateGenerationRuleInstance()
        {
            return Activator.CreateInstance(_type);
        }
    }
}