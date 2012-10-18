using System;
using System.Linq;

namespace BuildingBlocks.Query
{
    public class Pagination
    {
        public static Pagination<T> From<T>(IQueryable<T> itemQuery)
        {
            return new Pagination<T>(itemQuery);
        }
    }

    public class Pagination<T>
    {
        private IQueryable<T> _totalItemQuery;
        private int _pageNumber = 1;
        private int _pageSize = 100;
        private readonly IQueryable<T> _itemsSource;

        public Pagination(IQueryable<T> queryable)
        {
            _itemsSource = queryable;
            _totalItemQuery = queryable;
        }

        public Pagination<T> Page(int pageNumber, int pageSize)
        {
            _pageNumber = pageNumber;
            _pageSize = pageSize;
            return this;
        }

        public Pagination<T> TotalItemCountQueryFrom(IQueryable<T> items)
        {
            _totalItemQuery = items;
            return this;
        }

        public Page<T> GetPage()
        {
            return GetPageCore<T>(null);
        }

        public Page<TResult> GetPageWithItemsMappedBy<TResult>(Func<T, TResult> mapFunction)
        {
            return GetPageCore(mapFunction);
        }

        private Page<TResult> GetPageCore<TResult>(Func<T, TResult> mapFunction)
        {
            var totalItemCount = _totalItemQuery.Count();
            var page = new Page<TResult>(_pageNumber, _pageSize, totalItemCount);
            if (mapFunction == null)
            {
                page.QueryFrom((IQueryable<TResult>)_itemsSource);
            }
            else
            {
                page.QueryFrom(_itemsSource, mapFunction);
            }
            return page;
        }
    }
}