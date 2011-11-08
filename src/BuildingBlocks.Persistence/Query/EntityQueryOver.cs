using System;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NHibernate.Impl;

namespace BuildingBlocks.Persistence.Query
{
    public abstract class EntityQueryOver<TEntity> : QueryOver<TEntity, TEntity>
    {
        protected EntityQueryOver(Expression<Func<TEntity>> alias)
            : base(alias)
        {
        }

        protected EntityQueryOver()
        {
        }

        protected EntityQueryOver(string alias)
            : base(new CriteriaImpl(typeof(TEntity), alias, null))
        {
        }

        protected void SetAlias(string alias)
        {
            impl = new CriteriaImpl(typeof(TEntity), alias, null);
            criteria = impl;
        }

        protected void SetAlias(Expression<Func<TEntity>> alias)
        {
            var aliasPath = GetAliasPath(alias);
            impl = new CriteriaImpl(typeof(TEntity), aliasPath, null);
            criteria = impl;
        }

        protected string GetAliasPath(Expression<Func<TEntity>> alias)
        {
            return ExpressionProcessor.FindMemberExpression(alias.Body);
        }
    }
}