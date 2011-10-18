using System;
using System.Collections;

namespace BuildingBlocks.Persistence
{
    // ReSharper disable VirtualMemberNeverOverriden.Global
    public class Entity
    {
        private EntityPersistentState _persistentState = EntityPersistentState.New;

        public virtual bool IsPersistent
        {
            get { return _persistentState != EntityPersistentState.New; }
        }

        public virtual bool IsDeleted
        {
            get { return _persistentState == EntityPersistentState.Deleting || _persistentState == EntityPersistentState.DeleteCompleted; }
        }

        public virtual Type GetTypeUnproxied()
        {
            return GetType();
        }

        protected internal virtual void OnSave()
        {
            _persistentState = EntityPersistentState.Saved;
        }

        protected internal virtual void OnLoad()
        {
            _persistentState = EntityPersistentState.Loaded;
        }

        protected internal virtual void OnUpdate()
        {
            _persistentState = EntityPersistentState.Updated;
        }

        protected internal virtual void OnDelete()
        {
            //Contract.Requires(IsPersistent, "can delete only exists entities");
            _persistentState = EntityPersistentState.Deleting;
        }

        protected internal virtual void OnChangesSubmitted()
        {
            if (!IsPersistent)
                return;

            if (_persistentState == EntityPersistentState.Deleting)
            {
                _persistentState = EntityPersistentState.DeleteCompleted;
            }
        }

        protected bool DateTimeEquals(DateTime? dateTime1, DateTime? dateTime2)
        {
            var dateTimeComparer = new PersistenceDateTimeComparer();
            return dateTimeComparer.Equals(dateTime1, dateTime2);
        }

        public override bool Equals(object obj)
        {
            IEqualityComparer entityEqualityComparer = new GenericEntityEqualityComparer();
            return entityEqualityComparer.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    // ReSharper restore VirtualMemberNeverOverriden.Global
}