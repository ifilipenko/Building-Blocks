using System;
using System.Collections.Generic;

namespace BuildingBlocks.TestHelpers.Fixtures
{
    public class EnvironmentFixturePart : FixturePart
    {
        private readonly List<Action<object>> _instanceSetupActions;

        public EnvironmentFixturePart()
        {
            _instanceSetupActions = new List<Action<object>>();
        }

        internal IEnumerable<Action<object>> InstanceSetupActions
        {
            get { return _instanceSetupActions; }
        }

        protected void SetupInstance<TInstance>(Action<TInstance> action)
        {
            _instanceSetupActions.Add(x => action((TInstance)x));
        }
    }
}