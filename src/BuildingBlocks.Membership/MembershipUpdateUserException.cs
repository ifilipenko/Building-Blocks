using System;
using System.Runtime.Serialization;

namespace BuildingBlocks.Membership
{
    [Serializable]
    public class MembershipUpdateUserException : Exception
    {
        public MembershipUpdateUserException(string message)
            : base(message)
        {
        }

        public MembershipUpdateUserException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        protected MembershipUpdateUserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}