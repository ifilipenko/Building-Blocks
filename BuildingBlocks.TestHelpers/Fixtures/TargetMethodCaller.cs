using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.TestHelpers.Fixtures
{
    public class TargetMethodCaller<TTargetClass, TGiven>
        where TGiven : ParametersFixturePart
    {
        private readonly Action<object> _resultCallback;
        private readonly MethodInvokeContext<TTargetClass, TGiven> _context;

        public TargetMethodCaller(TTargetClass instance, TGiven given, Action<object> resultCallback)
        {
            _context = new MethodInvokeContext<TTargetClass, TGiven>(instance, given);
            _resultCallback = resultCallback;
        }

        public void Method(Action<MethodInvokeContext<TTargetClass, TGiven>> action)
        {
            MarkBeginMethod();
            action(_context);
            _resultCallback(null);
            MarkEndMethod();
        }

        public void Method(Func<MethodInvokeContext<TTargetClass, TGiven>, object> func)
        {
            MarkBeginMethod();
            var result = func(_context);
            _resultCallback(result);
            MarkEndMethod();
        }

        private static void MarkEndMethod()
        {
            Console.WriteLine("*********************** END METHOD ***********************");
        }

        private static void MarkBeginMethod()
        {
            Console.WriteLine("*********************** BEGIN METHOD ***********************");
        }

        public ExceptionsThrowAsserion WhenCall(Action<MethodInvokeContext<TTargetClass, TGiven>> action)
        {
            return new ExceptionsThrowAsserion(() => Method(action));
        }

        public ExceptionsThrowAsserion WhenCall(Func<MethodInvokeContext<TTargetClass, TGiven>, object> func)
        {
            return new ExceptionsThrowAsserion(() => Method(func));
        }

        private string ResolveTargetMethod(MethodBase currentMethod)
        {
            var words = currentMethod.Name.Split('_');
            if (words.Length < 2)
            {
                throw new InvalidOperationException("Test method has invalid name");
            }

            var methodNameWords = -1;
            for (int index = 0; index < words.Length; index++)
            {
                var word = words[index];
                if (word.ToLower() == "should")
                {
                    methodNameWords = index;
                    break;
                }
            }

            if (methodNameWords < 1)
            {
                return words[0];
            }

            var methodName = string.Join("_", words.Take(methodNameWords));
            return methodName;
        }

        private MethodBase ResolveCurrentTestMethod()
        {
            var frame = new StackFrame(2, false);
            var currentMethod = frame.GetMethod();
            var isTestMethod = currentMethod.GetCustomAttributes(false)
                .OfType<TestMethodAttribute>().Any();
            if (!isTestMethod)
            {
                throw new InvalidOperationException("Current method is not test method");
            }
            return currentMethod;
        }

        private object InvokeMethodOfTargetInstance(string methodName)
        {
            var method = _context.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new InvalidOperationException(string.Format("Method with name \"{0}\" is not exists or not public", methodName));
            }
            var result = method.Invoke(_context.Instance, _context.Given.Parameters.ToArray());
            return result;
        }
    }
}