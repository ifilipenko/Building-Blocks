using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Persistence.TestHelpers.TestData;

namespace BuildingBlocks.Persistence.TestHelpers
{
    public class PeristedEntitiesSet
    {
        protected internal readonly TestDataPersistor _db;

        public PeristedEntitiesSet(TestDataPersistor db)
        {
            _db = db;
        }

        public IEnumerable<T> Of<T>()
        {
            return _db.SavedObjects.OfType<T>().Distinct();
        }

        public IEnumerable<T> ExistsOnlyOf<T>()
            where T : class
        {
            return _db.SavedObjects.OfType<T>().Distinct().Where(e => _db.Exists(e));
        }
    }
}