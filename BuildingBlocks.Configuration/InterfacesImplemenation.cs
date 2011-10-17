using System;
using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Configuration
{
    public class InterfacesImplemenation
    {
        private readonly Type _type;
        private readonly IEnumerable<Type> _interfaces;

        public InterfacesImplemenation(Type type, IEnumerable<Type> interfaces)
        {
            Condition.Requires(type, "type").IsNotNull();
            Condition.Requires(interfaces, "interfaces").IsNotNull().IsNotEmpty();

            _type = type;
            _interfaces = interfaces;
        }

        public IEnumerable<Type> Interfaces
        {
            get { return _interfaces; }
        }

        public IEnumerable<Type> GetTargetObjectTypes()
        {
            return _interfaces.SelectMany(s => s.GetGenericArguments()).ToArray();
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}