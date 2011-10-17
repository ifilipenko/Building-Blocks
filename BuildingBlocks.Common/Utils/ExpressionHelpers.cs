using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BuildingBlocks.Common.Sugar;

namespace BuildingBlocks.Common.Utils
{
    public static class ExpressionHelpers
    {
        public static string GetMemberPath<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            return string.Join(".", expression.GetMembersPath().Select(p => p.Name).ToArray());
        }

        public static IEnumerable<MemberInfo> GetMembersPath(this Expression expression)
        {
            if (expression is LambdaExpression)
            {
                expression = ((LambdaExpression)expression).Body;
            }

            if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression) expression;
                if ((memberExpression.Expression.NodeType != ExpressionType.MemberAccess) &&
                    (memberExpression.Expression.NodeType != ExpressionType.Call))
                {
                    return new[] { memberExpression.Member };
                }

                var properties = memberExpression.Member.CastTo<MemberInfo>().ToEnumerable();
                var path = GetMembersPath(memberExpression.Expression).Concat(properties);
                return path;
            }

            if (expression is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)expression;
                if (unaryExpression.NodeType != ExpressionType.Convert)
                {
                    throw new Exception("Cannot interpret member from " + expression);
                }
                return GetMembersPath(unaryExpression.Operand);
            }
            throw new Exception("Unrecognised method call in expression " + expression);
        }

        [Obsolete("Use GetMemberPath")]
        public static string GetPropertyAccessString<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            return GetMemberPath(expression);
        }

        public static bool IsMemberExpression<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExp = RemoveUnary(expression.Body);
            return memberExp != null;
        }

        public static MemberInfo GetMemberInfo<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExp = RemoveUnary(expression.Body);

            return memberExp == null ? null : memberExp.Member;
        }

        public static MemberInfo GetMemberInfo(this LambdaExpression expression)
        {
            var memberExp = RemoveUnary(expression.Body);

            return memberExp == null ? null : memberExp.Member;
        }

        private static MemberExpression RemoveUnary(Expression toUnwrap)
        {
            if (toUnwrap is UnaryExpression)
            {
                return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
            }

            return toUnwrap as MemberExpression;
        }
    }
}
