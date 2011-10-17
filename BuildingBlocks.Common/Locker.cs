using System;

namespace BuildingBlocks.Common
{
    public class Locker
    {
        private bool _locked;

        public void DoAction(Action lockedAction)
        {
            if (lockedAction == null)
                throw new ArgumentNullException("lockedAction");

            if (_locked)
                return;

            _locked = true;
            try
            {
                lockedAction();
            }
            finally
            {
                _locked = false;
            }
        }

        public void DoEvent(object sender, EventHandler eventHandler)
        {
            if (eventHandler == null || _locked)
                return;

            _locked = true;
            try
            {
                eventHandler(sender, EventArgs.Empty);
            }
            finally
            {
                _locked = false;
            }
        }
    }
}
