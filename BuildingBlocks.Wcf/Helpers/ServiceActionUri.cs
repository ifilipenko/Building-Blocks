using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace BuildingBlocks.Wcf.Helpers
{
    [TypeConverter(typeof(UriTypeConverter))]
    [Serializable]
    public class ServiceActionUri : Uri
    {
        public ServiceActionUri(string uriString) 
            : base(uriString)
        {
        }

        public ServiceActionUri(string uriString, UriKind uriKind) 
            : base(uriString, uriKind)
        {
        }

        public ServiceActionUri(Uri baseUri, string relativeUri) 
            : base(baseUri, relativeUri)
        {
        }

        public ServiceActionUri(Uri baseUri, Uri relativeUri) 
            : base(baseUri, relativeUri)
        {
        }

        protected ServiceActionUri(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public string ServiceName
        {
            get
            {
                var segments = Segments;
                return segments.Length > 0 ? segments[1].Trim('/') : null;
            }
        }

        public string MethodName
        {
            get
            {
                var segments = Segments;
                return segments.Length > 1 ? segments[2].Trim('/') : null;
            }
        }
    }
}