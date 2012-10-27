using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace BuildingBlocks.Mvc.Helpers
{
    public static class RadioButtonListExtensions
    {
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList)
        {
            return RadioButtonList(htmlHelper, name, inputList, null);
        }

        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, object htmlAttributes)
        {
            return RadioButtonList(htmlHelper, name, inputList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList = null, IDictionary<string, object> htmlAttributes = null)
        {
            return RadioButtonListHelper(htmlHelper, name, inputList, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList, object htmlAttributes)
        {
            return RadioButtonListFor(htmlHelper, expression, inputList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes = null)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return RadioButtonListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), inputList, htmlAttributes);
        }

        private static MvcHtmlString RadioButtonListHelper(HtmlHelper htmlHelper, string name,
                                                           IEnumerable<SelectListItem> inputList,
                                                           IDictionary<string, object> htmlAttributes)
        {
            var listInputBuilder = new ListInputBuilder(htmlHelper)
                                       {
                                           Name = name,
                                           AllowMultiple = false,
                                           HtmlAttributes = htmlAttributes,
                                           InputList = inputList
                                       };
            return listInputBuilder.Render();
        }
    }
}