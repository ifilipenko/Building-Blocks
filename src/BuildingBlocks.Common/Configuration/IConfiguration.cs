namespace BuildingBlocks.Common.Configuration
{
    public interface IConfiguration
    {
        string GetSetting(string name);
        T LoadTo<T>()
            where T : new();
    }
}