using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Persistence.TestHelpers.TestData;
using BuildingBlocks.TestHelpers.DataGenerator;
using BuildingBlocks.TestHelpers.DataGenerator.Randomizer;

namespace BuildingBlocks.Persistence.TestHelpers
{
    public abstract class DataScenario
    {
        private readonly DataGenerator _dataGenerator;
        private readonly RandomValues _randomValues;
        private readonly TestDataPersistor _db;
        private readonly PeristedEntitiesSet _entities;

        protected DataScenario(DataGenerator dataGenerator)
        {
            _dataGenerator = dataGenerator;
            _randomValues = new RandomValues();
            _db = new TestDataPersistor();
            _entities = new PeristedEntitiesSet(_db);
        }

        protected DataGenerator DataGenerator
        {
            get { return _dataGenerator; }
        }

        public TestDataPersistor Db
        {
            get { return _db; }
        }

        public T RandomElement<T>(IEnumerable<T> enumerable)
        {
            var count = enumerable.Count();
            var index = _randomValues.RandomInt(0, count);
            return enumerable.ElementAt(index);
        }

        public PeristedEntitiesSet Entities
        {
            get { return _entities; } 
        }

        public abstract void Run();
    }

    public abstract class DataScenario<TParameters> : DataScenario
        where TParameters : new()
    {
        private readonly TParameters _parameters;

        protected DataScenario(DataGenerator dataGenerator)
            : base(dataGenerator)
        {
            _parameters = new TParameters();
        }

        public TParameters Parameters
        {
            get { return _parameters; }
        }
    }
}