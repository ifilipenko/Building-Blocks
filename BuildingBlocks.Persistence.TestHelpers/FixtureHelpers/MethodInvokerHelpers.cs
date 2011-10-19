using System;
using BuildingBlocks.TestHelpers.Fixtures;

namespace BuildingBlocks.Persistence.TestHelpers.FixtureHelpers
{
    public static class TargetMethodCallerHelpers
    {
        public static void MethodInUowScope<TTarget, TGiven>(this TargetMethodCaller<TTarget, TGiven> methodCaller, Action<MethodInvokeContext<TTarget, TGiven>> method) 
            where TGiven : ParametersFixturePart
        {
            using (var uow = UnitOfWork.TransactionScope())
            {
                methodCaller.Method(method);
                uow.SubmitChanges();
            }
        }

        public static void MethodInUowScope<TTarget, TGiven>(this TargetMethodCaller<TTarget, TGiven> methodCaller, Func<MethodInvokeContext<TTarget, TGiven>, object> method)
            where TGiven : ParametersFixturePart
        {
            using (var uow = UnitOfWork.TransactionScope())
            {
                methodCaller.Method(method);
                uow.SubmitChanges();
            }
        }
    }
}