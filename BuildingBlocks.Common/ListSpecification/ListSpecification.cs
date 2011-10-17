using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BuildingBlocks.Common.ListSpecification.Nodes;
using BuildingBlocks.Common.Utils;

namespace BuildingBlocks.Common.ListSpecification
{
    public class ListSpecification<T> : IListSpecification<T>
    {
        protected List<SortEntry> _sortExpressions;
        private int? _pageIndex;
        private int? _pageSize;
        private GroupOperation _groupOperation;

        public ListSpecification()
        {
            _sortExpressions = new List<SortEntry>();
        }

        public Type EntityType
        {
            get { return typeof (T); }
        }

        public virtual int? PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        public virtual int? PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public virtual FilterNode Filter { get; set; }

        public virtual IEnumerable<SortEntry> Orders
        {
            get { return _sortExpressions; }
            protected set { _sortExpressions = value.AsList(); }
        }

        public virtual GroupOperation GroupOperation
        {
            get { return _groupOperation; }
            set
            {
                _groupOperation = value;
                var filter = Filter as GroupFilterNode;
                if (filter != null)
                {
                    filter.GroupOperation = value;
                }
            }
        }

        public bool PagingEnabled
        {
            get { return PageIndex.HasValue && PageSize.HasValue; }
        }

        public IListSpecification<T> AddEqualTo(Expression<Func<T, object>> property, object value)
        {
            return Add(property, CompareOperator.EqualTo, value);
        }

        public IListSpecification<T> AddGreaterThen(Expression<Func<T, object>> property, object value)
        {
            return Add(property, CompareOperator.GreaterThen, value);
        }

        public IListSpecification<T> AddLessThen(Expression<Func<T, object>> property, object value)
        {
            return Add(property, CompareOperator.LessThen, value);
        }

        public IListSpecification<T> AddNotEqualTo(Expression<Func<T, object>> property, object value)
        {
            return Add(property, CompareOperator.NotEqualTo, value);
        }

        public IListSpecification<T> AddContains(Expression<Func<T, object>> property, object value)
        {
            return Add(property, CompareOperator.Contains, value);
        }

        public IListSpecification<T> Add(Expression<Func<T, object>> property, CompareOperator compareOperator, object value)
        {
            return Add(property.GetMemberPath(), compareOperator, value);
        }

        public IListSpecification<T> Add(string property, CompareOperator compareOperator, object value)
        {
            var propertyFilter = new FilterEntry
                                     {
                                         Property = property,
                                         CompareOperator = compareOperator,
                                         Value = value
                                     };
            Add(propertyFilter);
            return this;
        }

        public void Add(FilterEntry filterEntry)
        {
            SetFilter(filterEntry.Property, filterEntry.CompareOperator, filterEntry.Value);
        }

        private void SetFilter(string property, CompareOperator compareOperator, object value)
        {
            if (Filter == null)
            {
                Filter = new GroupFilterNode(GroupOperation, Enumerable.Empty<FilterNode>());
            }
            ((GroupFilterNode) Filter).AddNode(new PropertyValueFilterNode(property, compareOperator, value));
        }

        public IListSpecification<T> OrderBy(string property)
        {
            if (string.IsNullOrEmpty(property)) 
                throw new ArgumentNullException("property");

            var propertySort = new SortEntry {Property = property, IsDescending = false};
            _sortExpressions.Add(propertySort);
            return this;
        }

        public IListSpecification<T> OrderBy(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            var propertySort = new SortEntry {Property = property.GetMemberInfo().Name, IsDescending = false};
            _sortExpressions.Add(propertySort);
            return this;
        }

        public IListSpecification<T> OrderByDescending(string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException("property");
            var propertySort = new SortEntry {Property = property, IsDescending = true};
            _sortExpressions.Add(propertySort);
            return this;
        }

        public IListSpecification<T> OrderByDescending(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            var propertySort = new SortEntry {Property = property.GetMemberInfo().Name, IsDescending = true};
            _sortExpressions.Add(propertySort);
            return this;
        }

        public IListSpecification<T> Page(int pageIndex, int pageSize)
        {
            _pageIndex = pageIndex;
            _pageSize = pageSize;
            return this;
        }
    }
}