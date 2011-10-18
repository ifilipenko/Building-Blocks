using System;
using BuildingBlocks.Common.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildingBlocks.TestHelpers.Fixtures
{
    [TestClass]
    public abstract class FixtureBase<TargetClassInstance, TWhen, TGiven, TThen> : AbstractTest
        where TWhen : EnvironmentFixturePart, new()
        where TGiven : ParametersFixturePart, new()
        where TThen : AssertionsFixturePart, new()
    {
        private DependenciesManager _mocks;
        private TWhen _when;
        private TGiven _given;
        private TThen _then;
        private ValueReference<object> _resultRef;
        private TargetClassInstance _newInstance;

        protected abstract TargetClassInstance SetupTargetInstance(DependenciesManager dependencies);

        protected TargetMethodCaller<TargetClassInstance, TGiven> Do
        {
            get
            {
                _newInstance = SetupTargetInstance(Mocks);
                foreach (var action in When.InstanceSetupActions)
                {
                    action(_newInstance);
                }
                return new TargetMethodCaller<TargetClassInstance, TGiven>(_newInstance, Given, r => _resultRef.Value = r);
            }
        }

        protected virtual TWhen When
        {
            get
            {
                return _when ?? (_when = new TWhen
                                             {
                                                 Dependencies = Mocks,
                                                 DataGenerator = DataGenerator
                                             });
            }
        }

        protected virtual TGiven Given
        {
            get
            {
                return _given ?? (_given = new TGiven
                                               {
                                                   Dependencies = Mocks,
                                                   DataGenerator = DataGenerator
                                               });
            }
        }

        protected virtual TThen Then
        {
            get
            {
                if (!_resultRef.WasInitialized)
                {
                    throw new InvalidOperationException("Before start validation need invoke target method");
                }
                return _then ?? (_then = new TThen
                                             {
                                                 Instance = _newInstance,
                                                 Dependencies = Mocks,
                                                 DataGenerator = DataGenerator,
                                                 Result = _resultRef.Value
                                             });
            }
        }

        private DependenciesManager Mocks
        {
            get { return _mocks ?? (_mocks = new DependenciesManager()); }
        }
    }
}