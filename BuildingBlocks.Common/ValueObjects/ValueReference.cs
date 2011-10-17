namespace BuildingBlocks.Common.ValueObjects
{
    public struct ValueReference<TValue>
    {
        private TValue _value;
        private bool _wasInitialized;

        public ValueReference(TValue value)
        {
            _wasInitialized = true;
            _value = value;
        }

        public bool WasInitialized
        {
            get { return _wasInitialized; }
        }

        public TValue Value
        {
            get { return _value; }
            set
            {
                _wasInitialized = true;
                _value = value;
            }
        }
    }
}