using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BuildingBlocks.Store.RavenDB
{
    public static class RavenDbQueryHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="property"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Expected property or field getter</exception>
        public static IQueryable<T> PropertyContains<T, TValue>(this IQueryable<T> queryable, Expression<Func<T, string>> property, IEnumerable<TValue> values)
        {
            var memberExpression = (MemberExpression) property.Body;
            if (memberExpression.Member.MemberType != MemberTypes.Field && memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("Expected property or field getter", "property");
            }
            return PropertyContains(queryable, memberExpression.Member.Name, values);
        }

        public static IQueryable<T> PropertyContains<T, TValue>(this IQueryable<T> queryable, string property, IEnumerable<TValue> values)
        {
            if (!values.Any())
                return queryable;

            var criterions = values.Select(name => property + " = \"" + name + "\"");
            var expression = string.Join(" || ", criterions);
            return queryable.Where(expression);
        }
    }
}