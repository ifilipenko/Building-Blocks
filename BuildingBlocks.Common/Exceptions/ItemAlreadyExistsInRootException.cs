namespace BuildingBlocks.Common.Exceptions
{
    public class ItemAlreadyExistsInRootException : AggregateRootException 
    {
        const string MessageText = "Item already exists in root object";

        public override string Message
        {
            get
            {
                return MessageText;
            }
        }
    }
}