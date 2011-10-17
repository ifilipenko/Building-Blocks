using System;

namespace BuildingBlocks.Common.XmlLoader
{
    [Serializable]
    public class XMLLoaderException : Exception
    {
        public XMLLoaderException() { }
        public XMLLoaderException(string message) : base(message) { }
        public XMLLoaderException(string message, Exception inner) : base(message, inner) { }
        protected XMLLoaderException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class NotFoundRequaredNodeException : XMLLoaderException
    {
        public NotFoundRequaredNodeException() { }
        public NotFoundRequaredNodeException(string message) : base(message) { }
        public NotFoundRequaredNodeException(string message, Exception inner) : base(message, inner) { }
        protected NotFoundRequaredNodeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}