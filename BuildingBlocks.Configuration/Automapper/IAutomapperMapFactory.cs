using AutoMapper;

namespace BuildingBlocks.Configuration.Automapper
{
    public interface IAutomapperMapFactory<T>
    {
        IMappingExpression<T, TDest> CreateMapTo<TDest>();
        IMappingExpression<TSrc, T> CreateMapFrom<TSrc>();
    }
}