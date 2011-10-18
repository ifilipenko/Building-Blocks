using System;
using CuttingEdge.Conditions;
using FluentAssertions;

namespace BuildingBlocks.TestHelpers.Fixtures
{
    public class ExceptionsThrowAsserion
    {
        private readonly Action _action;

        public ExceptionsThrowAsserion(Action action)
        {
            Condition.Requires(action, "action").IsNotNull();
            
            _action = action;
        }

        public void ThrowException()
        {
            _action.ShouldThrow<Exception>();
        }

        public void ThrowException(string reason, params object[] reasonParameters)
        {
            _action.ShouldThrow<Exception>(reason, reasonParameters);
        }

        public void Throw<TException>() 
            where TException : Exception
        {
            _action.ShouldThrow<TException>();
        }

        public void Throw<TException>(string reason, params object[] reasonParameters)
            where TException : Exception
        {
            _action.ShouldThrow<TException>(reason, reasonParameters);
        }

        public void NotThrowException()
        {
            _action.ShouldNotThrow<Exception>();
        }

        public void NotThrowException(string reason, params object[] reasonParameters)
        {
            _action.ShouldNotThrow<Exception>(reason, reasonParameters);
        }

        public void NotThrow<TException>()
            where TException : Exception
        {
            _action.ShouldNotThrow<TException>();
        }

        public void NotThrow<TException>(string reason, params object[] reasonParameters)
            where TException : Exception
        {
            _action.ShouldNotThrow<TException>(reason, reasonParameters);
        }
    }
}