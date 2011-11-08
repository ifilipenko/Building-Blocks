using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common.Automapper;

namespace BuildingBlocks.Common
{
    public interface IPagedList<T> : IList<T>
    {
        new int Count { get; }
        long TotalCount { get; }
        int PageIndex { get; set; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        int PageCount { get; }
        int StartIndex { get; }
        int EndIndex { get; }
    }

    /// <summary>
    /// Paged list
    /// </summary>
    /// <typeparam name="T">item type</typeparam>
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList(IEnumerable<T> pageData, int pageIndex, int pageSize, int totalItems)
        {
            AddRange(pageData);

            TotalCount = totalItems;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public PagedList(int pageIndex, int pageSize, int totalItems)
        {
            TotalCount = totalItems;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public PagedList(IEnumerable pageData, Func<object, T> pageItemConvertor, int pageIndex, int pageSize, long totalItems)
        {
            foreach (var pageItem in pageData)
            {
                T item = pageItemConvertor(pageItem);
                Add(item);
            }

            TotalCount = totalItems;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public long TotalCount { get; private set; }
        public int PageSize { get; private set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex - 1) > 0; }
        }

        public bool HasNextPage
        {
            get { return ((PageIndex + 1)*PageSize) < TotalCount; }
        }

        public int PageIndex { get; set; }

        public int StartIndex
        {
            get { return TotalCount == 0 ? 0 : PageIndex*PageSize; }
        }

        public int EndIndex
        {
            get { return TotalCount == 0 ? 0 : StartIndex + this.Count() - 1; }
        }

        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    throw new InvalidOperationException("Page size equal 0");

                if (TotalCount == 0)
                {
                    return 0;
                }

                int remainder = (int) (TotalCount%PageSize);
                return (int) (remainder == 0 ? TotalCount/PageSize : (TotalCount/PageSize) + 1);
            }
        }
    }

    public static class Pagination
    {
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> pageData, int index, int pageSize, int totalCount)
        {
            return new PagedList<T>(pageData, index, pageSize, totalCount);
        }

        public static PagedList<TTarget> MapToPagedList<TSource, TTarget>(this IEnumerable<TSource> pageData, int index, int pageSize, long totalCount) 
            where TSource : class 
            where TTarget : class
        {
            return new PagedList<TTarget>(pageData, src => ((TSource) src).Map<TSource, TTarget>(), index, pageSize, totalCount);
        }
    }
}