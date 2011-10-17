using System;
using System.Collections.Generic;
using System.Reflection;

namespace BuildingBlocks.Common
{
    public class TypesFinder
    {
        private readonly Assembly _assembly;

        public TypesFinder(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            _assembly = assembly;
        }

        public IList<Type> FindClassesImplementedInterface(Type targetInterface)
        {
            if (targetInterface == null)
            {
                throw new ArgumentNullException("targetInterface");
            }
            if (!targetInterface.IsInterface)
            {
                throw new ArgumentException("Expected interface but was " + targetInterface, "targetInterface");
            }

            return _FindImplementedInterfaceTypes(targetInterface, false);
        }

        public IList<Type> FindInterfacesImplementedInterface(Type targetInterface)
        {
            if (targetInterface == null)
            {
                throw new ArgumentNullException("targetInterface");
            }
            if (!targetInterface.IsInterface)
            {
                throw new ArgumentException("Expected interface but was " + targetInterface, "targetInterface");
            }

            return _FindImplementedInterfaceTypes(targetInterface, true);
        }

        private IList<Type> _FindImplementedInterfaceTypes(Type targetInterface, bool interfaces)
        {
            List<Type> result = new List<Type>();
            foreach (Type type in _assembly.GetTypes())
            {
                if ((type.IsInterface == interfaces) && (type.GetInterface(targetInterface.Name) != null))
                {
                    result.Add(type);
                }
            }
            return result.AsReadOnly();
        }

        public IList<Type> FindSubclassesOf(Type targetBaseClass)
        {
            if (targetBaseClass == null)
            {
                throw new ArgumentNullException("targetBaseClass");
            }
            if (targetBaseClass.IsInterface)
            {
                throw new ArgumentException("Expected class but was interface " + targetBaseClass, "targetBaseClass");
            }
            if (targetBaseClass.IsValueType)
            {
                throw new ArgumentException("Expected class but was value type " + targetBaseClass, "targetBaseClass");
            }

            List<Type> result = new List<Type>();
            foreach (Type type in _assembly.GetTypes())
            {
                if (type.IsSubclassOf(targetBaseClass))
                {
                    result.Add(type);
                }
            }
            return result.AsReadOnly();
        }
    }
}