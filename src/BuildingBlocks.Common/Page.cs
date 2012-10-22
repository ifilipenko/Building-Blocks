using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common
{
    public class Page<T>
    {
        public static Page<T> Empty(int pageNumber, int pageSize)
        {
            return new Page<T>(pageNumber, pageSize, 0)
                {
                    Items = new List<T>(0)
                };
        }

        public Page(int pageNumber, int pageSize, long totalItemCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
        }

        public IList<T> Items { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public long TotalItemCount { get; private set; }

        public int PageIndex
        {
            get { return PageNumber - 1; }
        }

        public int PageCount
        {
            get { return TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0; }
        }

        public bool HasPreviousPage
        {
            get { return PageNumber > 1; }
        }

        public bool HasNextPage
        {
            get { return PageNumber < PageCount; }
        }

        public bool IsFirstPage
        {
            get { return PageNumber == 1; }
        }

        public bool IsLastPage
        {
            get { return PageNumber >= PageCount; }
        }

        public long FirstItemOnPage
        {
            get { return (PageNumber - 1) * PageSize + 1; }
        }

        public long LastItemOnPage
        {
            get
            {
                long num = FirstItemOnPage + PageSize - 1;
                return num > TotalItemCount ? TotalItemCount : num;
            }
        }

        public int SkippedItems
        {
            get
            {
                return PageNumber == 1 ? 0 : (PageNumber - 1) * PageSize;
            }
        }

        public void QueryFrom<TSource>(IQueryable<TSource> itemsSource, Func<TSource, T> mapFunction)
        {
            var sources = itemsSource.Skip(SkippedItems).Take(PageSize).ToList();
            Items = sources.Select(mapFunction).ToList();
        }

        public void QueryFrom(IQueryable<T> itemsSource)
        {
            Items = itemsSource.Skip(SkippedItems).Take(PageSize).ToList();
        }

        public Page<TResult> MapWith<TResult>(Func<T, TResult> mapFunction)
        {
            return new Page<TResult>(PageNumber, PageSize, TotalItemCount)
                {
                    Items = Items.Select(mapFunction).ToList()
                };
        }
    }
}