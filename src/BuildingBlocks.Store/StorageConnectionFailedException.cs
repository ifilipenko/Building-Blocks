using System;
using System.Runtime.Serialization;

namespace BuildingBlocks.Store
{
    [Serializable]
    public class StorageConnectionFailedException : Exception
    {
        private const string DefaultMessage = "Connection failed";

        public StorageConnectionFailedException(Exception inner)
            : base(DefaultMessage, inner)
        {
        }

        public StorageConnectionFailedException(string message) 
            : base(message)
        {
        }

        public StorageConnectionFailedException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        protected StorageConnectionFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}