namespace BuildingBlocks.Common.Configuration
{
    public interface IApplicationConfiguraion
    {
        string GetSetting(string name);
        T DeserializeTo<T>() where T : new();
    }
}