using System;
using System.Collections.Generic;
using BuildingBlocks.Common;

namespace BuildingBlocks.Persistence.Specification
{
    public interface IListSpecificationQueryOverExecution<T>
    {
        IList<T> ToList(bool applyDistinctRoot = false);
        IList<TResult> ToListOf<TResult>(bool applyDistinctRoot = false);
        IPagedList<T> ToPagedList(bool applyDistinctRoot = false);
        IPagedList<TResult> ToPagedListOf<TResult>(bool applyDistinctRoot = false);
        IPagedList<TResult> ToMappedPagedList<TResult>(Func<T, TResult> map, bool applyDistinctRoot = false);
    }
}