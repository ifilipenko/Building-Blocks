namespace BuildingBlocks.Common.Configuration
{
    public interface IConfiguraion
    {
        string GetSetting(string name);
        T LoadTo<T>()
            where T : new();
    }
}