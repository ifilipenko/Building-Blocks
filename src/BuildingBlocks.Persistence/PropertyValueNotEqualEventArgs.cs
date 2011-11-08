namespace BuildingBlocks.Persistence
{
    public class PropertyValueCompareResultEventArgs
    {
        public string PropertyPath { get; set; }
        public object ValueFromSource { get; set; }
        public object ValueFromOther { get; set; }
        public bool Result { get; set; }
    }
}