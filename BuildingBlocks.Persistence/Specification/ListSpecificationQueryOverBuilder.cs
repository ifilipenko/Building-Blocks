using System.Collections;
using System.Linq;
using BuildingBlocks.Common.ListSpecification;
using BuildingBlocks.Common.ListSpecification.Nodes;
using NHibernate;
using NHibernate.Criterion;

namespace BuildingBlocks.Persistence.Specification
{
    class ListSpecificationQueryOverBuilder
    {
        private readonly IQueryOver _query;

        public ListSpecificationQueryOverBuilder(IQueryOver query)
        {
            _query = query;
        }

        public IQueryOver Query
        {
            get { return _query; }
        }

        private ICriteria RootCriteria
        {
            get { return _query.RootCriteria; }
        }

        public void ApplyOrders(IListSpecification listSpecification)
        {
            if (listSpecification.Orders == null)
                return;

            foreach (var order in listSpecification.Orders)
            {
                RootCriteria.AddOrder(new Order(order.Property, !order.IsDescending));
            }
        }

        public void ApplyFilter(IListSpecification listSpecification)
        {
            if (listSpecification.Filter == null) 
                return;

            var clearInvalidNodes = new InvalidNodesCleaner();
            var filter = clearInvalidNodes.Process(listSpecification.Filter);

            var nodeToCriteriaConverter = new FilterNodeToCriteriaConverter();
            var criterion = nodeToCriteriaConverter.ToCriterion(filter);
            if (criterion != null)
            {
                RootCriteria.Add(criterion);
            }
        }

        class FilterNodeToCriteriaConverter : IFilterNodeVisitor<ICriterion>
        {
            public ICriterion ToCriterion(FilterNode filterNode)
            {
                return filterNode.Accept(this);
            }

            #region Implementation of IFilterNodeVisitor

            ICriterion IFilterNodeVisitor<ICriterion>.Visit(GroupFilterNode node)
            {
                var criterions = node.Children
                    .Select(filterNode => filterNode.Accept(this))
                    .Where(c => c != null)
                    .ToList();
                if (criterions.Count == 0)
                    return null;

                var junction = node.GroupOperation == GroupOperation.Or
                                   ? (Junction) new Disjunction()
                                   : new Conjunction();
                foreach (var criterion in criterions)
                {
                    junction.Add(criterion);
                }

                return junction;
            }

            ICriterion IFilterNodeVisitor<ICriterion>.Visit(IsNullFilterNode node)
            {
                return Restrictions.IsNull(node.PropertyName);
            }

            ICriterion IFilterNodeVisitor<ICriterion>.Visit(NotFilterNode node)
            {
                var criterion = node.InnerNode.Accept(this);
                return criterion == null ? null : Restrictions.Not(criterion);
            }

            ICriterion IFilterNodeVisitor<ICriterion>.Visit(PropertyValueFilterNode node)
            {
                if (node.Value == null)
                {
                    return new IsNullFilterNode(node.PropertyName).Accept(this);
                }

                switch (node.CompareOperator)
                {
                    case CompareOperator.EqualTo:
                        return Restrictions.Eq(node.PropertyName, node.Value);
                    case CompareOperator.GreaterThen:
                        return Restrictions.Gt(node.PropertyName, node.Value);
                    case CompareOperator.GreaterOrEqual:
                        return Restrictions.Ge(node.PropertyName, node.Value);
                    case CompareOperator.LessThen:
                        return Restrictions.Lt(node.PropertyName, node.Value);
                    case CompareOperator.LessOrEqual:
                        return Restrictions.Le(node.PropertyName, node.Value);
                    case CompareOperator.NotEqualTo:
                        return Restrictions.Not(Restrictions.Eq(node.PropertyName, node.Value));
                    case CompareOperator.Contains:
                        return ContainsCriteria(node);
                    case CompareOperator.NotContains:
                        return Restrictions.Not(ContainsCriteria(node));
                    case CompareOperator.In:
                        return InCriterion(node.PropertyName, node.Value);
                    case CompareOperator.StartsWith:
                        return Restrictions.Like(node.PropertyName, ValueToString(node), MatchMode.Start);
                    case CompareOperator.EndsWith:
                        return Restrictions.Like(node.PropertyName, ValueToString(node), MatchMode.End);
                }

                return null;
            }

            private static SimpleExpression ContainsCriteria(PropertyValueFilterNode node)
            {
                return Restrictions.Like(node.PropertyName, ValueToString(node), MatchMode.Anywhere);
            }

            #endregion

            private ICriterion InCriterion(string propertyName, object value)
            {
                object[] valueArray = null;
                if (value is IList)
                {
                    valueArray = ((IEnumerable) value).OfType<object>().ToArray();
                }

                return valueArray == null
                           ? Restrictions.Eq(propertyName, value)
                           : Restrictions.In(propertyName, valueArray);
            }

            private static string ValueToString(PropertyValueFilterNode node)
            {
                return node.Value is string ? (string)node.Value : node.Value.ToString();
            }
        }
    }
}