using System;
using System.Reflection;

namespace BuildingBlocks.Common.XmlLoader
{
    class NodeToChildMapper
    {
        private readonly Type _ownerClassToLoad;
        private readonly PropertyInfo _propertyInfo;

        protected NodeToChildMapper(Type ownerClassToLoad, PropertyInfo propertyInfo)
        {
            _ownerClassToLoad = ownerClassToLoad;
            _propertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }

        public Type OwnerClassToLoad
        {
            get { return _ownerClassToLoad; }
        }
    }
}