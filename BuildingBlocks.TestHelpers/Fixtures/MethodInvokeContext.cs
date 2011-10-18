namespace BuildingBlocks.TestHelpers.Fixtures
{
    public class MethodInvokeContext<TTargetClass, TGiven>
    {
        private readonly TTargetClass _instance;
        private readonly TGiven _given;

        public MethodInvokeContext(TTargetClass instance, TGiven given)
        {
            _instance = instance;
            _given = given;
        }

        public TGiven Given
        {
            get { return _given; }
        }

        public TTargetClass Instance
        {
            get { return _instance; }
        }
    }
}