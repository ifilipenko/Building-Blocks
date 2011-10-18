using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BuildingBlocks.Common.Utils;
using NHibernate.Linq;

namespace BuildingBlocks.Persistence.Fetching
{
    public class Fetch
    {
        private readonly LambdaExpression _relatedObjectSelector;
        private readonly PropertyInfo _relatedMember;
        private readonly FetchCollection _relatedFetches;

        public Fetch(LambdaExpression relatedObjectSelector)
        {
            _relatedObjectSelector = relatedObjectSelector;
            _relatedMember = GetPropertyFromExpression(relatedObjectSelector);
            _relatedFetches = new FetchCollection();
        }

        public IEnumerable<Fetch> RelatedFetches
        {
            get { return _relatedFetches; }
        }

        internal FetchCollection RelatedFetchesCollection
        {
            get { return _relatedFetches; }
        }

        public PropertyInfo Member
        {
            get { return _relatedMember; }
        }

        public IQueryable<T> ApplyFetch<T>(IQueryable<T> queryable)
        {
            var root = _relatedObjectSelector.Parameters.First().Type;
            if (root != typeof(T))
            {
                throw new InvalidOperationException("Can not apply fetching from root [" + root + "] to query of [" + typeof(T) + "]");
            }

            var fetchRequest = ApplyFetchCore(queryable, string.Empty);
            foreach (var relatedFetch in _relatedFetches)
            {
                relatedFetch.ApplyFetchFromChild(fetchRequest);
            }
            return fetchRequest;
        }

        private void ApplyFetchFromChild<T>(IQueryable<T> queryable)
        {
            ApplyFetchCore(queryable, "Then");
        }

        private IQueryable<T> ApplyFetchCore<T>(IQueryable<T> queryable, string fetchMethodPrefix)
        {
            string fetchMethodName;
            Type relationType;
            if (typeof (IEnumerable).IsAssignableFrom(_relatedMember.PropertyType))
            {
                fetchMethodName = fetchMethodPrefix + "FetchMany";
                relationType = GetCollectionItemType(_relatedMember.PropertyType);
            }
            else
            {
                fetchMethodName = fetchMethodPrefix + "Fetch";
                relationType = _relatedMember.PropertyType;
            }

            var requestCreateMethod = GetType()
                .GetMethod("CreateFluentFetchRequest", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(T), relationType);
            var fetchRequest = requestCreateMethod.Invoke(this, new object[] {fetchMethodName, queryable});

            return (IQueryable<T>) fetchRequest;
        }

        // ReSharper disable UnusedMember.Local
        private INhFetchRequest<T, TRelated> CreateFluentFetchRequest<T, TRelated>(
            string fetchMethodName, IQueryable<T> query)
        {
            switch (fetchMethodName)
            {
                case "FetchMany":
                    return query.FetchMany((Expression<Func<T, IEnumerable<TRelated>>>) _relatedObjectSelector);
                case "ThenFetch":
                case "ThenFetchMany":
                    var genericArguments = query.GetType().GetGenericArguments();
                    var fetchType = genericArguments.Last();
                    var fetchMethod = typeof (EagerFetchingExtensionMethods).GetMethod(fetchMethodName)
                        .MakeGenericMethod(typeof(T), fetchType, typeof(TRelated));
                    return new NhFetchRequest<T, TRelated>(query.Provider, Expression.Call(
                        fetchMethod,
                        new[] {query.Expression, _relatedObjectSelector}));
                default:
                    return query.Fetch((Expression<Func<T, TRelated>>) _relatedObjectSelector);
            }            
        }
        // ReSharper restore UnusedMember.Local

        // ReSharper disable PossibleNullReferenceException
        private Type GetCollectionItemType(Type type)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition().GetInterface(typeof(IEnumerable<>).Name) != null)
            {
                return type.GetGenericArguments().First();
            }
            throw new InvalidOperationException("Can not extract element type from type" + type);
        }
        // ReSharper restore PossibleNullReferenceException

        private static PropertyInfo GetPropertyFromExpression(LambdaExpression relatedObjectSelector)
        {
            var memberInfo = relatedObjectSelector.GetMemberInfo();
            if (memberInfo.MemberType != MemberTypes.Property)
                throw new ArgumentException("Unexpected member type " + memberInfo, "relatedObjectSelector");
            return (PropertyInfo) memberInfo;
        }

        public void CopyRelatedFetchesFrom(Fetch fetch)
        {
            if (fetch.Member != Member)
                throw new ArgumentException("Expected instance fetch for member " + Member + " but was " + fetch.Member);

            _relatedFetches.MergeWithFetchesFrom(fetch.RelatedFetches);
        }

        public void AddFetch(Fetch fetch)
        {
            _relatedFetches.Add(fetch);
        }
    }
}