namespace BuildingBlocks.TestHelpers.Mocks
{
    public class Setup<TRepository> : MocksMethods
    {
        internal protected TRepository Repository { get; set; }
    }
}