using System;
using System.Runtime.Serialization;

namespace BuildingBlocks.Membership
{
    [Serializable]
    public class MembershipProviderException : Exception
    {
        public MembershipProviderException()
        {
        }

        public MembershipProviderException(string message) : base(message)
        {
        }

        public MembershipProviderException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MembershipProviderException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}