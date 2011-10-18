using System.Linq;
using BuildingBlocks.Common.Sugar;
using NHibernate;
using NHibernate.Type;

namespace BuildingBlocks.Persistence
{
    class EntityInterceptor : EmptyInterceptor
    {
        public override bool? IsTransient(object entity)
        {
            if (entity is Entity)
            {
                return !((Entity) entity).IsPersistent;
            }
            return base.IsTransient(entity);
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            entity.WhenIsOf<Entity>(e => e.OnUpdate());
            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            entity.WhenIsOf<Entity>(e => e.OnLoad());
            return base.OnLoad(entity, id, state, propertyNames, types);
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            entity.WhenIsOf<Entity>(e => e.OnSave());
            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            entity.WhenIsOf<Entity>(e => e.OnDelete());
            base.OnDelete(entity, id, state, propertyNames, types);
        }

        public override void PostFlush(System.Collections.ICollection entities)
        {
            base.PostFlush(entities);

            foreach (var entity in entities.OfType<Entity>())
            {
                entity.OnChangesSubmitted();
            }
        }
    }
}