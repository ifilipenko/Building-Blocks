using System;

namespace BuildingBlocks.Common.Utils
{
    public class SwitchType
    {
        public static SwitchType ForInstance(object instance)
        {
            if (instance == null) 
                throw new ArgumentNullException("instance");
            return new SwitchType(instance);
        }

        private readonly object _instance;
        private bool _caseMatched;
        private object _result;

        private SwitchType(object instance)
        {
            _instance = instance;
        }

        public ISwitchTypeOf<TResult> ExpecteResultOf<TResult>()
        {
            return new SwitchTypeOf<TResult>(_instance);
        }

        public interface ISwitchTypeOf<TResult>
        {
            ISwitchTypeOf<TResult> CaseOf<T>(Func<T, TResult> caseBody);
            TResult GetResult();
        }

        public SwitchType CaseOf<T>(Func<T, object> caseBody)
        {
            InvokeMethodOnTypeMatched(caseBody);
            return this;
        }

        public object GetResult()
        {
            if (!_caseMatched)
                throw new InvalidOperationException("No case matched");
            return _result;
        }

        public object GetResultOrDefault()
        {
            if (!_caseMatched)
                return null;
            return _result;
        }

        private void InvokeMethodOnTypeMatched<T>(Func<T, object> caseBody)
        {
            if (caseBody == null)
                throw new ArgumentNullException("caseBody");

            if (_caseMatched)
                return;

            _caseMatched = _instance is T;
            if (_caseMatched)
            {
                _result = caseBody((T)_instance);
            }
        }

        class SwitchTypeOf<TResult> : SwitchType, ISwitchTypeOf<TResult>
        {
            public SwitchTypeOf(object instance)
                : base(instance)
            {
            }

            #region Implementation of ISwitchTypeOf<TResult>

            public ISwitchTypeOf<TResult> CaseOf<T>(Func<T, TResult> caseBody)
            {
                Func<T, object> caseBodaAdapter = i => caseBody(i);
                InvokeMethodOnTypeMatched(caseBodaAdapter);
                return this;
            }

            public new TResult GetResult()
            {
                return (TResult) base.GetResult();
            }

            public new TResult GetResultOrDefault()
            {
                var result = base.GetResultOrDefault();
                return (TResult) (result ?? default(TResult));
            }

            #endregion
        }
    }
}