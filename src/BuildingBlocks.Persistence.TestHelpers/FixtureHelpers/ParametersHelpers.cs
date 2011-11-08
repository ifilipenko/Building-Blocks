using System.Linq;
using NHibernate.Criterion;

namespace BuildingBlocks.Persistence.TestHelpers.FixtureHelpers
{
    public static class ParametersHelpers
    {
        public static long not_exists_id_for<TEntity>(this ParametersBase parameters)
            where TEntity : class
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var maxId = repository.QueryOver<TEntity>()
                    .SelectList(p => p.Select(Projections.Max(Projections.Id())))
                    .SingleOrDefault<long>();
                return maxId + 1;
            }
        }

        public static long first_id_of<TEntity>(this ParametersBase parameters)
            where TEntity : class
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var id = repository.QueryOver<TEntity>()
                    .OrderBy(Projections.Id()).Asc
                    .SelectList(p => p.Select(Projections.Id()))
                    .Take(1)
                    .SingleOrDefault<long>();
                return id;
            }
        }

        public static TEntity first_entity_of<TEntity>(this ParametersBase parameters)
            where TEntity : class
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                return repository.Query<TEntity>().First();
            }
        }

        public static TEntity first_entity_with_id<TEntity>(this ParametersBase parameters, long id)
            where TEntity : class
        {
            using (UnitOfWork.Scope())
            {
                var repository = new Repository();
                var entity = repository.QueryOver<TEntity>()
                    .Where(Restrictions.IdEq(id))
                    .SingleOrDefault();
                return entity;
            }
        }
    }
}
