using System;

namespace BuildingBlocks.Common.XmlLoader
{
    [Serializable]
    public class XMLLoaderUnqueCheckException : Exception
    {
        public XMLLoaderUnqueCheckException() { }
        public XMLLoaderUnqueCheckException(string message) : base(message) { }
        public XMLLoaderUnqueCheckException(string message, Exception inner) : base(message, inner) { }
        protected XMLLoaderUnqueCheckException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}