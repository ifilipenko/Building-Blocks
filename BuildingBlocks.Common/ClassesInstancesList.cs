using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace BuildingBlocks.Common
{
    public class ClassesInstancesList<TBase>
    {
        private readonly Dictionary<Type, TBase> _instances = new Dictionary<Type, TBase>();

        public IEnumerable Instances
        {
            get { return _instances.Values; }
        }

        public TargetType GetInstance<TargetType>()
            where TargetType : TBase
        {
            TBase result = default(TBase);
            try
            {
                result = _instances[typeof (TargetType)];
                if (Equals(result, default(TBase)))
                {
                    RemoveInstance<TargetType>();
                }
            }
            catch (KeyNotFoundException)
            {
            }
            return (TargetType) result;
        }

        public void RemoveInstance<TargetType>()
        {
            _instances.Remove(typeof (TargetType));
        }

        public void Add<TargetType>(TargetType instance)
            where TargetType : TBase
        {
            Debug.Assert(!Equals(instance, default(TargetType)));

            try
            {
                _instances[typeof(TargetType)] = instance;
            }
            catch (KeyNotFoundException)
            {
                _instances.Add(typeof(TargetType), instance);
            }
        }

        public bool ContainsInstance(TBase instance)
        {
            Debug.Assert(!Equals(instance, default(TBase)));

            foreach (object existingInstance in Instances)
            {
                if (ReferenceEquals(instance, existingInstance))
                {
                    return true;
                }
            }

            return false;
        }
    }
}