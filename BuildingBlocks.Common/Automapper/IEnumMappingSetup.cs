namespace BuildingBlocks.Common.Automapper
{
    public interface IEnumMappingSetup<TDest>
    {
        void Ignore<T>(T sourceEnumValue)
            where T : struct;
        IEnumValueMappingSetup<TDest> Source<T>(T sourceEnumValue)
            where T : struct;

    }

    public interface IEnumValueMappingSetup<TDest>
    {
        IEnumMappingSetup<TDest> MapTo(TDest destinationEnumValue);
    }
}