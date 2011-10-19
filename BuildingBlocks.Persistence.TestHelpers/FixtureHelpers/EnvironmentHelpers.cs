namespace BuildingBlocks.Persistence.TestHelpers.FixtureHelpers
{
    public static class EnvironmentHelpers
    {
        public static void exists_one_of<T>(this EnvironmentBase environment)
        {
            var entity = environment.DataGenerator.Single<T>().Get();
            environment.Db.Persist(entity);
        }
    }
}