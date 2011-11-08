namespace BuildingBlocks.TestHelpers.Mocks
{
    public abstract class MockRepository<TConcreteMockRepository, TWhen, TVerify>
        where TConcreteMockRepository : MockRepository<TConcreteMockRepository, TWhen, TVerify>
        where TWhen : Setup<TConcreteMockRepository>, new()
        where TVerify : Verification<TConcreteMockRepository>, new()
    {
        private readonly TVerify _verifyThat;
        private readonly TWhen _when;

        protected MockRepository()
        {
            _when = new TWhen();
            _verifyThat = new TVerify();
            _when.Repository = (TConcreteMockRepository) this;
            _verifyThat.Repository = (TConcreteMockRepository)this;
        }

        public TWhen When
        {
            get { return _when; }
        }

        public TVerify VerifyThat
        {
            get { return _verifyThat; }
        }
    }
}