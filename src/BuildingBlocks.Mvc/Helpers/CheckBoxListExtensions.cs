using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace BuildingBlocks.Mvc.Helpers
{
    public static class CheckBoxListExtensions 
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList)
        {
            return CheckBoxList(htmlHelper, name, inputList, null);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, object htmlAttributes)
        {
            return CheckBoxList(htmlHelper, name, inputList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList = null, IDictionary<string, object> htmlAttributes = null)
        {
            return CheckBoxListHelper(htmlHelper, name, inputList, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList, object htmlAttributes)
        {
            return CheckBoxListFor(htmlHelper, expression, inputList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return CheckBoxListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), inputList, htmlAttributes);
        }

        private static MvcHtmlString CheckBoxListHelper(HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> inputList, IDictionary<string, object> htmlAttributes)
        {
            var listInputBuilder = new ListInputBuilder(htmlHelper)
            {
                Name = name,
                AllowMultiple = true,
                HtmlAttributes = htmlAttributes,
                InputList = inputList
            };
            return listInputBuilder.Render();
        }
        
    }
}