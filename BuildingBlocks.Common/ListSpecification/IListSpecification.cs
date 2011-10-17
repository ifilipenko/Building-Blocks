using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BuildingBlocks.Common.ListSpecification.Nodes;

namespace BuildingBlocks.Common.ListSpecification
{
    public enum GroupOperation
    {
        And,
        Or
    }

    public interface IListSpecification
    {
        Type EntityType { get; }

        int? PageIndex { get; }
        int? PageSize { get; }
        bool PagingEnabled { get; }

        FilterNode Filter { get; set; }
        IEnumerable<SortEntry> Orders { get; }
        GroupOperation GroupOperation { get; set; }
    }

    public interface IListSpecification<T> : IListSpecification
    {
        IListSpecification<T> AddEqualTo(Expression<Func<T, object>> property, object value);
        IListSpecification<T> AddGreaterThen(Expression<Func<T, object>> property, object value);
        IListSpecification<T> AddLessThen(Expression<Func<T, object>> property, object value);
        IListSpecification<T> AddNotEqualTo(Expression<Func<T, object>> property, object value);
        IListSpecification<T> AddContains(Expression<Func<T, object>> property, object value);
        
        IListSpecification<T> Add(Expression<Func<T, object>> property, CompareOperator compareOperator, object value);
        IListSpecification<T> Add(string property, CompareOperator compareOperator, object value);

        IListSpecification<T> OrderBy(string property);
        IListSpecification<T> OrderBy(Expression<Func<T, object>> property);

        IListSpecification<T> OrderByDescending(string property);
        IListSpecification<T> OrderByDescending(Expression<Func<T, object>> property);

        IListSpecification<T> Page(int pageIndex, int pageSize);
    }
}