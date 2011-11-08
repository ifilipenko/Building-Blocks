using System;
using System.Collections.Generic;
using System.Reflection;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Configuration
{
    public class InterfacesSelector
    {
        private readonly AssembliesListBuilder _assembliesListBuilder;
        private readonly HashSet<Assembly> _assembliesSet;
        private Func<Type, bool> _filter;

        public InterfacesSelector()
        {
            _assembliesSet = new HashSet<Assembly>();
            _assembliesListBuilder = new AssembliesListBuilder(_assembliesSet);
        }

        internal Func<Type, bool> InterfaceFilter
        {
            get { return _filter; }
        }

        internal HashSet<Assembly> AssembliesSet
        {
            get { return _assembliesSet; }
        }

        public AssembliesListBuilder Assemblies
        {
            get { return _assembliesListBuilder; }
        }

        public void OnlyMatchedTo(Func<Type, bool > predicate)
        {
            Condition.Requires(predicate, "predicate").IsNotNull();
            _filter = predicate;
        }
    }
}