using System.Collections.Generic;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    class ObjectParsedValues 
    {
        private readonly List<ObjectValue> _associations = new List<ObjectValue>();
        private readonly List<ObjectValue> _components = new List<ObjectValue>();
        private readonly List<ObjectValue> _collections = new List<ObjectValue>();
        private readonly List<ObjectValue> _primives = new List<ObjectValue>();

        public IEnumerable<ObjectValue> Associations
        {
            get { return _associations; }
        }

        public IEnumerable<ObjectValue> Components
        {
            get { return _components; }
        }

        public IEnumerable<ObjectValue> Collections
        {
            get { return _collections; }
        }

        public IEnumerable<ObjectValue> Primives
        {
            get { return _primives; }
        }

        public void AddAssociation(ObjectValue value)
        {
            if (value == null)
                return;
            _associations.Add(value);
        }

        public void AddComponents(ObjectValue value)
        {
            if (value == null)
                return;
            _components.Add(value);
        }

        public void AddCollection(ObjectValue value)
        {
            if (value == null)
                return;
            _collections.Add(value);
        }

        public void AddPrimitive(ObjectValue value)
        {
            if (value == null)
                return;
            _primives.Add(value);
        }
    }
}