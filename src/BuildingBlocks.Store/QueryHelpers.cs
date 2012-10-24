using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace BuildingBlocks.Store
{
    public static class QueryHelpers
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
        public static IQueryable<T> ContainsIn<T, TValue>(this IQueryable<T> queryable, Expression<Func<T, TValue>> property, IEnumerable<TValue> values)
            where TValue : struct
        {
            var memberExpression = GetMemberExpression(property);
            return ContainsIn(queryable, memberExpression.Member.Name, values);
        }

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
        public static IQueryable<T> ContainsIn<T>(this IQueryable<T> queryable, Expression<Func<T, string>> property, IEnumerable<string> values)
        {
            var memberExpression = GetMemberExpression(property);
            return ContainsIn(queryable, memberExpression.Member.Name, values);
        }

        public static IQueryable<T> ContainsIn<T>(this IQueryable<T> queryable, string property, IEnumerable<string> values)
        {
            return PropertyContainsCore(queryable, property, values);
        }

        public static IQueryable<T> ContainsIn<T, TValue>(this IQueryable<T> queryable, string property, IEnumerable<TValue> values)
            where TValue : struct
        {
            return PropertyContainsCore(queryable, property, values);
        }

        private static IQueryable<T> PropertyContainsCore<T, TValue>(IQueryable<T> queryable, string property, IEnumerable<TValue> values)
        {
            if (!values.Any())
                return queryable;

            var wrapper = typeof(TValue) == typeof(string) ? "\"" : string.Empty;
            var criterions = values.Select(name => property + " = " + wrapper + name + wrapper);
            var expression = string.Join(" || ", criterions);
            return queryable.Where(expression);
        }

        private static MemberExpression GetMemberExpression<T, TValue>(Expression<Func<T, TValue>> property)
        {
            var memberExpression = (MemberExpression) property.Body;
            if (memberExpression.Member.MemberType != MemberTypes.Field &&
                memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("Expected property or field getter", "property");
            }
            return memberExpression;
        }
    }
}