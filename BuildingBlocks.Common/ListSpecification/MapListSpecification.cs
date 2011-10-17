using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BuildingBlocks.Common.ListSpecification.Nodes;
using BuildingBlocks.Common.Reflection;
using BuildingBlocks.Common.Utils;
using BuildingBlocks.Common.Utils.Hierarchy;

namespace BuildingBlocks.Common.ListSpecification
{
    public class MapListSpecification<TSource, TTarget> : ListSpecification<TTarget>
    {
        private readonly IListSpecification<TSource> _sourceListSpecification;
        private readonly List<string> _excludedProperties;
        private readonly Dictionary<string, Func<FilterEntry, FilterEntry[]>> _propertiesMap;

        public MapListSpecification(IListSpecification<TSource> sourceListSpecification)
        {
            if (sourceListSpecification == null) 
                throw new ArgumentNullException("sourceListSpecification");

            _sourceListSpecification = sourceListSpecification;
            _propertiesMap = new Dictionary<string, Func<FilterEntry, FilterEntry[]>>();
            _excludedProperties = new List<string>(0);
        }

        public override IEnumerable<SortEntry> Orders
        {
            get
            {
                var orders = new List<SortEntry>();
                var sourceOrders = _sourceListSpecification.Orders.Where(o => !IsExcluded(o.Property));
                foreach (var sourceOrder in sourceOrders)
                {
                    Func<FilterEntry, FilterEntry[]> mapFunc;
                    if (_propertiesMap.TryGetValue(sourceOrder.Property, out mapFunc))
                    {
                        var entries = mapFunc(new FilterEntry {Property = sourceOrder.Property});
                        orders.RemoveAll(o => entries.Any(e => e.Property == o.Property));
                        orders.AddRange(entries.Select(e => new SortEntry
                                                                {
                                                                    IsDescending = sourceOrder.IsDescending,
                                                                    Property = e.Property
                                                                }));
                    }
                    else if (!orders.Any(o => o.Property == sourceOrder.Property))
                    {
                        orders.Add(sourceOrder);
                    }
                }

                orders.RemoveAll(o => _sortExpressions.Any(x => x.Property == o.Property));
                orders.AddRange(_sortExpressions);

                return orders;
            }
            protected set
            {
                _sortExpressions = value.AsList();
            }
        }

        public override GroupOperation GroupOperation
        {
            get
            {
                return _sourceListSpecification.GroupOperation;
            }
        }

        public override FilterNode Filter
        {
            get
            {
                var filterTransformVisitor = new FilterTransformVisitor(_excludedProperties, _propertiesMap);
                var resultFilter = filterTransformVisitor.Process(_sourceListSpecification.Filter);
                return resultFilter;
            }
            set
            {
                throw new NotSupportedException("Can not set filter for mapped specification");
            }
        }

        public override int? PageIndex
        {
            get
            {
                return _sourceListSpecification.PageIndex;
            }
            set
            {
            }
        }

        public override int? PageSize
        {
            get
            {
                return _sourceListSpecification.PageSize;
            }
            set
            {
            }
        }

        public MapListSpecification<TSource, TTarget> MapProperty(string sourceProperty, Func<FilterEntry, FilterEntry[]> mapFunction)
        {
            if (string.IsNullOrEmpty(sourceProperty))
                throw new ArgumentNullException("sourceProperty");
            if (mapFunction == null)
                throw new ArgumentNullException("mapFunction");
            
            _propertiesMap[sourceProperty] = mapFunction;
            return this;
        }

        public MapListSpecification<TSource, TTarget> MapPropertyByFunc<TProp>(
            Expression<Func<TSource, TProp>> sourceProperty,
            Func<FilterEntry, FilterEntry[]> mapFunction)
        {
            if (sourceProperty == null)
                throw new ArgumentNullException("sourceProperty");
            if (mapFunction == null)
                throw new ArgumentNullException("mapFunction");

            return MapProperty(sourceProperty.GetMemberPath(), mapFunction);
        }

        public MapListSpecification<TSource, TTarget> MapPropertyByFunc<TProp>(
            Expression<Func<TSource, TProp>> sourceProperty,
            Func<FilterEntry, FilterEntry> mapFunction)
        {
            if (sourceProperty == null)
                throw new ArgumentNullException("sourceProperty");
            if (mapFunction == null)
                throw new ArgumentNullException("mapFunction");

            return MapProperty(sourceProperty.GetMemberPath(), e => new[] { mapFunction(e) });
        }

        public MapListSpecification<TSource, TTarget> MapProperty(string sourceProperty, string targetProperty,
            Func<CompareOperator, CompareOperator> compareOperatorConverter = null, 
            Func<object, object> valueConverter = null)
        {
            if (string.IsNullOrEmpty(sourceProperty))
                throw new ArgumentNullException("sourceProperty");
            if (string.IsNullOrEmpty(targetProperty))
                throw new ArgumentNullException("targetProperty");

            if (valueConverter == null)
            {
                valueConverter = NullValueConverter;
            }
            if (compareOperatorConverter == null)
            {
                compareOperatorConverter = NullCompareOperatorConverter;
            }

            Func<FilterEntry, FilterEntry[]> mapFunction = sourceEntry =>
            {
                Debug.Assert(sourceEntry != null);
                Debug.Assert(sourceEntry.Property == sourceProperty);

                var entry = new FilterEntry
                                {
                                    Property = targetProperty,
                                    CompareOperator = compareOperatorConverter(sourceEntry.CompareOperator),
                                    Value = valueConverter(sourceEntry.Value)
                                };
                return new[] {entry};
            };
            _propertiesMap.AddOrUpdate(sourceProperty, mapFunction);

            return this;
        }

        public MapListSpecification<TSource, TTarget> MapProperty<TProp>(
            Expression<Func<TSource, TProp>> sourceProperty,
            Expression<Func<TTarget, TProp>> targetProperty)
        {
            if (sourceProperty == null) 
                throw new ArgumentNullException("sourceProperty");
            if (targetProperty == null) 
                throw new ArgumentNullException("targetProperty");

            return MapProperty(sourceProperty.GetMemberPath(), targetProperty.GetMemberPath());
        }

        public MapListSpecification<TSource, TTarget> MapProperty<TSourceValue, TTargetValue>(
            Expression<Func<TSource, TSourceValue>> sourceProperty,
            Expression<Func<TTarget, TTargetValue>> targetProperty,
            Func<TSourceValue, TTargetValue> valueConverter)
        {
            return MapProperty(sourceProperty, targetProperty, null, valueConverter);
        }

        public MapListSpecification<TSource, TTarget> MapProperty<TSourceValue, TTargetValue>(
            Expression<Func<TSource, TSourceValue>> sourceProperty,
            Expression<Func<TTarget, TTargetValue>> targetProperty,
            Func<CompareOperator, CompareOperator> compareOperatorConverter,
            Func<TSourceValue, TTargetValue> valueConverter)
        {
            if (sourceProperty == null)
                throw new ArgumentNullException("sourceProperty");
            if (targetProperty == null)
                throw new ArgumentNullException("targetProperty");
            if (valueConverter == null)
                throw new ArgumentNullException("valueConverter");

            var valueConverterAdapter = new Func<object, object>(x => valueConverter((TSourceValue)x));
            return MapProperty(sourceProperty.GetMemberPath(), targetProperty.GetMemberPath(), compareOperatorConverter, valueConverterAdapter);
        }

        public MapListSpecification<TSource, TTarget> Exclude(Expression<Func<TSource, object>> sourceProperty)
        {
            if (sourceProperty == null) 
                throw new ArgumentNullException("sourceProperty");
            return Exclude(sourceProperty.GetMemberPath());
        }

        private MapListSpecification<TSource, TTarget> Exclude(string memberPath)
        {
            if (string.IsNullOrEmpty(memberPath)) 
                throw new ArgumentNullException("memberPath");

            _excludedProperties.Add(memberPath);
            return this;
        }

        public void ValidatePropertiesMapping()
        {
            var allProperties = typeof (TTarget).GetProperties();

            var filter = Filter;
            if (filter != null)
            {
                foreach (var node in filter.FlattenHierarchy().OfType<PropertyFilterNode>())
                {
                    ValidatePropertyName(allProperties, node.PropertyName);
                }
            }
            foreach (var order in Orders)
            {
                ValidatePropertyName(allProperties, order.Property);
            }
        }

        private static void ValidatePropertyName(IEnumerable<PropertyInfo> allProperties, string propertyName)
        {
            var propertyExists = allProperties.Any(p => p.Name == propertyName);
            if (propertyExists)
                return;

            try
            {
                    var propertyPath = propertyName.GetPropertyPath<TTarget>();
                    if (propertyPath.Length == 0)
                    {
                        throw new InvalidPropertyFromListSpecificationMappingException(typeof(TSource),
                                                                                       typeof(TTarget), propertyName);
                    }
            }
            catch (Exception ex)
            {
                throw new InvalidPropertyFromListSpecificationMappingException(typeof (TSource), typeof (TTarget),
                                                                               propertyName, ex);
            }
        }

        private bool IsExcluded(string property)
        {
            return _excludedProperties.Contains(property);
        }

        private CompareOperator NullCompareOperatorConverter(CompareOperator compareOperator)
        {
            return compareOperator;
        }

        private object NullValueConverter(object value)
        {
            return value;
        }

        private class FilterTransformVisitor : IFilterNodeVisitor<FilterNode>
        {
            private readonly IEnumerable<string> _excludedProperties;
            private readonly IDictionary<string, Func<FilterEntry, FilterEntry[]>> _propertiesMap;

            public FilterTransformVisitor(IEnumerable<string> excludedProperties, IDictionary<string, Func<FilterEntry, FilterEntry[]>> propertiesMap)
            {
                _excludedProperties = excludedProperties;
                _propertiesMap = propertiesMap;
            }

            public FilterNode Process(FilterNode filterNode)
            {
                return filterNode == null ? null : filterNode.Accept(this);
            }

            FilterNode IFilterNodeVisitor<FilterNode>.Visit(GroupFilterNode node)
            {
                if (node.IsInvalid)
                    return null;

                var childrenNodes = node.Children.Select(Process).Where(n => n != null && !n.IsInvalid).ToList();
                if (childrenNodes.Count == 0)
                    return null;
                return childrenNodes.Count == 1
                           ? childrenNodes.First()
                           : new GroupFilterNode(node.GroupOperation, childrenNodes);
            }

            FilterNode IFilterNodeVisitor<FilterNode>.Visit(IsNullFilterNode node)
            {
                return node.IsInvalid ? null : MapNode(node, FilterEntryToIsNullFilterNode);
            }

            FilterNode IFilterNodeVisitor<FilterNode>.Visit(NotFilterNode node)
            {
                if (node.IsInvalid)
                    return null;

                var innerNode = Process(node.InnerNode);
                return innerNode == null || innerNode.IsInvalid ? null : new NotFilterNode(innerNode);
            }

            FilterNode IFilterNodeVisitor<FilterNode>.Visit(PropertyValueFilterNode node)
            {
                return node.IsInvalid ? null : MapNode(node, FilterEntryToPropertyValueFilterNode);
            }

            private FilterNode MapNode(PropertyFilterNode sourceNode, Func<FilterEntry, FilterNode> entryToNodeConverter)
            {
                if (_excludedProperties.Contains(sourceNode.PropertyName))
                {
                    return null;
                }

                Func<FilterEntry, FilterEntry[]> map;
                if (!_propertiesMap.TryGetValue(sourceNode.PropertyName, out map))
                    return sourceNode.Clone();

                var filterEntries = map(sourceNode.ToFilterEntry());
                if (filterEntries.Length == 0)
                    return null;

                if (filterEntries.Length > 1)
                {
                    return new GroupFilterNode(GroupOperation.And, filterEntries.Select(entryToNodeConverter));
                }

                var newFilterNode = (PropertyFilterNode) sourceNode.Clone();
                newFilterNode.FillByFilterEntry(filterEntries.First());

                return newFilterNode;
            }

            private static PropertyValueFilterNode FilterEntryToPropertyValueFilterNode(FilterEntry e)
            {
                return new PropertyValueFilterNode(e.Property, e.CompareOperator, e.Value);
            }

            private static IsNullFilterNode FilterEntryToIsNullFilterNode(FilterEntry e)
            {
                return new IsNullFilterNode(e.Property);
            }
        }
    }
}