using System;
using System.Collections.Generic;
using System.Reflection;
using BuildingBlocks.Common.Utils;

namespace BuildingBlocks.Common
{
    public class PropertyEvalChain
    {
        private readonly Type _ownerClass;
        private readonly IList<PropertyInfo> _evaluationChain;

        public PropertyEvalChain(Type ownerClass, IList<PropertyInfo> chain)
        {
            if (ownerClass == null)
            {
                throw new ArgumentNullException("ownerClass");
            }
            if (chain == null)
            {
                throw new ArgumentNullException("chain");
            }

            _ownerClass = ownerClass;
            _evaluationChain = chain;
        }

        public object EvaluateValue(object ownedClassInstance)
        {
            if (ownedClassInstance == null)
            {
                throw new ArgumentNullException("ownedClassInstance");
            }
            if (ownedClassInstance.GetType() != _ownerClass)
            {
                throw new ArgumentException(string.Format("Expected value of class [{0}] but was [{1}]", _ownerClass, ownedClassInstance.GetType()));
            }

            object value = ownedClassInstance;
            foreach (var property in _evaluationChain)
            {
                value = property.GetValue(value, null);
            }
            return value;
        }

        public string ChainToString()
        {
            return CollectionUtil.ItemsToString(_evaluationChain, ".", 
                                                delegate(object item) { return ((PropertyInfo) item).Name; });
        }

        public override string ToString()
        {
            string enity = _evaluationChain.Count == 0
                               ? "empty"
                               : CollectionUtil.ItemsToString(_evaluationChain, ", ");
            return string.Format("{0} {{ {1} }}", GetType().Name, enity);
        }
    }
}