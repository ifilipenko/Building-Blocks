using System;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Fasterflect;
using PerpetuumSoft.Knockout;

namespace BuildingBlocks.Mvc.KnockoutMvc
{
    public static class KnockoutBindingHelpers
    {
        public static KnockoutBinding<TModel> Href<TModel>(this KnockoutBinding<TModel> knockoutBinding, Expression<Func<TModel, string>> urlGetter)
        {
            var attrItem = (KnockoutBingindComplexItem)knockoutBinding.CallMethod("ComplexItem", "attr");
            var item = new EvaluatedValueKnockoutBindingItem
            {
                Name = "href",
                ValueExpression = data => GetServerUrlExpression(data, urlGetter)
            };
            attrItem.Add(item);
            return knockoutBinding;
        }

        public static KnockoutBinding<TModel> Href<TModel>(this KnockoutBinding<TModel> knockoutBinding, string action, string controller)
        {
            return Href(knockoutBinding, action, controller, null, null);
        }

        public static KnockoutBinding<TModel> Href<TModel>(this KnockoutBinding<TModel> knockoutBinding, string action, string controller, string parameter, Expression<Func<TModel, object>> urlParametersGetter)
        {
            var attrItem = (KnockoutBingindComplexItem) knockoutBinding.CallMethod("ComplexItem", "attr");
            var item = new EvaluatedValueKnockoutBindingItem
            {
                Name = "href",
                ValueExpression = data => GetServerUrlExpression(data, action, controller, parameter, urlParametersGetter)
            };
            attrItem.Add(item);
            return knockoutBinding;
        }

        public static KnockoutBinding<TModel> ClickEvent<TModel>(this KnockoutBinding<TModel> knockoutBinding, Expression<Func<TModel, string>> urlGetter)
        {
            var item = new ServerActionKnockoutBindingItem
                {
                    Name = "click",
                    UrlExpression = data => GetServerUrlExpression(data, urlGetter)
                };
            
            knockoutBinding.Items.Add(item);
            return knockoutBinding;
        }

        public static KnockoutBinding<TModel> ClickEvent<TModel>(this KnockoutBinding<TModel> knockoutBinding, string action, string controller)
        {
            return ClickEvent(knockoutBinding, action, controller, null, null);
        }

        public static KnockoutBinding<TModel> ClickEvent<TModel>(this KnockoutBinding<TModel> knockoutBinding, string action, string controller, string parameter, Expression<Func<TModel, object>> urlParametersGetter)
        {
            var item = new ServerActionKnockoutBindingItem
                {
                    Name = "click",
                    UrlExpression = data => GetServerUrlExpression(data, action, controller, parameter, urlParametersGetter)
                };

            knockoutBinding.Items.Add(item);
            return knockoutBinding;
        }

        private class EvaluatedValueKnockoutBindingItem : KnockoutBindingItem
        {
            public Func<KnockoutExpressionData, string> ValueExpression { get; set; }

            public override string GetKnockoutExpression(KnockoutExpressionData data)
            {
                if (ValueExpression == null)
                    throw new InvalidOperationException("Value expression is miss");

                var stringBuilder = new StringBuilder();
                stringBuilder.Append(Name);
                stringBuilder.Append(" : ");
                stringBuilder.Append(ValueExpression(data));
                return stringBuilder.ToString();
            }
        }

        private class ServerActionKnockoutBindingItem : KnockoutBindingItem
        {
            public Func<KnockoutExpressionData, string> UrlExpression { get; set; }

            public sealed override string GetKnockoutExpression(KnockoutExpressionData data)
            {
                if (UrlExpression == null)
                    throw new InvalidOperationException("Url expression is miss");

                var stringBuilder = new StringBuilder();
                stringBuilder.Append(Name);
                stringBuilder.Append(" : ");
                stringBuilder.Append("function() {");
                stringBuilder.AppendFormat("executeOnServer($root, {0})", UrlExpression(data));
                stringBuilder.Append(";}");
                return stringBuilder.ToString();
            }
        }

        private static string GetUrl(string action, string controller, string parameterName, string parameterValueScript)
        {
            const string parameterPlaceholder = "__A5B7A8749D6__";
            var url = GetUrlHelper().Action(action, controller, new RouteValueDictionary { { parameterName, parameterPlaceholder } });
            return url.EndsWith(parameterPlaceholder)
                       ? "'" + url.Replace(parameterPlaceholder, "' + " + parameterValueScript)
                       : "'" + url.Replace(parameterPlaceholder, "' + " + parameterPlaceholder + " + '");
        }

        private static UrlHelper GetUrlHelper()
        {
            return new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));
        }

        private static string GetServerUrlExpression<TModel>(KnockoutExpressionData data, Expression<Func<TModel, string>> urlGetter)
        {
            var expression = KnockoutExpressionConverter.Convert(urlGetter, data);
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new InvalidOperationException("Url getter is invalid");
            }
            return expression;
        }

        private static string GetServerUrlExpression<TModel>(KnockoutExpressionData data, string action, string controller, string parameterName, Expression<Func<TModel, object>> parametersGetter)
        {
            if (!string.IsNullOrWhiteSpace(parameterName) && parametersGetter == null)
                throw new InvalidOperationException("Url parameter getter is miss");

            string expression;
            if (parametersGetter == null)
            {
                expression = "'" + GetUrlHelper().Action(action, controller) + "'";
            }
            else
            {
                var getterExpression = KnockoutExpressionConverter.Convert(parametersGetter, data);
                if (string.IsNullOrWhiteSpace(getterExpression))
                {
                    throw new InvalidOperationException("Url parameter getter is invalid");
                }

                if (parametersGetter.Body.NodeType == ExpressionType.MemberAccess)
                {
                    getterExpression += "()";
                }
                expression = GetUrl(action, controller, parameterName, getterExpression);
            }
            return expression;
        }
    }
}