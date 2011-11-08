using AutoMapper;

namespace BuildingBlocks.Configuration.Automapper
{
    public class AutomapperMapFactory<T> : IAutomapperMapFactory<T>
    {
        public IMappingExpression<T, TDest> CreateMapTo<TDest>()
        {
            return Mapper.CreateMap<T, TDest>();
        }

        public IMappingExpression<TSrc, T> CreateMapFrom<TSrc>()
        {
            return Mapper.CreateMap<TSrc, T>();
        }
    }
}