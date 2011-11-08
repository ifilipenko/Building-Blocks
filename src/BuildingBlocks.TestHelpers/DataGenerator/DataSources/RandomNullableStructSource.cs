using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.Randomizer;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class RandomNullableStructSource<T> : IDatasource<T?>
        where T : struct 
    {
        private static readonly RandomValues _randomValues = new RandomValues();

        public object Next(IGenerationContext context)
        {
            var isNull = _randomValues.Random<bool>();
            return isNull ? (T?) null : _randomValues.Random<T>();
        }
    }
}