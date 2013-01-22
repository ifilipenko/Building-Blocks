using AutoPoco.Engine;
using BuildingBlocks.Common;

namespace BuildingBlocks.Autopoco.Helpers
{
    public static class GeneratorHelpers
    {
        public static Page<T> PageOf<T>(this IGenerationSession generator, int pageNumber, int pageSize, int totalCount)
        {
            var items = generator.List<T>(pageSize).Get();
            var page = new Page<T>(pageNumber, pageSize, totalCount);
            page.SetItems(items);
            return page;
        }
    }
}