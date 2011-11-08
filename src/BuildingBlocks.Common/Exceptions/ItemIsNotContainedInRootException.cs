namespace BuildingBlocks.Common.Exceptions
{
    public class ItemIsNotContainedInRootException : AggregateRootException 
    {
        const string MessageText = "Item is not contained in root object";

        public override string Message
        {
            get
            {
                return MessageText;
            }
        }
    }
}