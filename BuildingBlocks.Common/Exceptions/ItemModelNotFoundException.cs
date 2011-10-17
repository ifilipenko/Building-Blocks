using System;

namespace BuildingBlocks.Common.Exceptions
{
    public class ItemModelNotFoundException : AggregateRootException
    {
        private readonly Type _itemType;

        public ItemModelNotFoundException(Type itemType)
        {
            _itemType = itemType;
        }

        public Type ItemType
        {
            get { return _itemType; }
        }

        const string MessageText = "Item model for type is not setted";

        public override string Message
        {
            get
            {
                return string.Format(MessageText, _itemType);
            }
        }
    }
}