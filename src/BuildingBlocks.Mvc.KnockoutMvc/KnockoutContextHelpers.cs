using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildingBlocks.Mvc.KnockoutMvc
{
    public static class KnockoutContextHelpers
    {
        private const string ScriptEndTag = "</script>";
        private const string ScriptStartTag = "<script type=\"text/javascript\">";

        public static KnockoutForeachContext<TItem> ForeachEnumerable<TModel, TItem>(this KnockoutContext<TModel> context, Expression<Func<TModel, IEnumerable<TItem>>> binding)
        {
            var viewContext = (ViewContext) context.GetFieldValue("viewContext");
            var knockoutForeachContext = new KnockoutForeachContext<TItem>(
                viewContext, 
                KnockoutExpressionConverter.Convert(binding, context.CreateData())
            );
            knockoutForeachContext.WriteStart(viewContext.Writer);

            var contextStackProperty = context.GetType().Property("ContextStack");
            var contextStack = (List<IKnockoutContext>) contextStackProperty.GetValue(context);
            contextStackProperty.SetValue(context, contextStack);

            contextStack.Add(knockoutForeachContext);
            return knockoutForeachContext;
        }

        public static HtmlString ApplyWithOutScriptTag<TModel>(this KnockoutContext<TModel> context, TModel model, string bindingElementSelector = null)
        {
            var htmlString = context.Apply(model);
            var script = htmlString.ToString();

            var scriptStartTagIndex = script.IndexOf(ScriptStartTag);
            script = script.Remove(scriptStartTagIndex, ScriptStartTag.Length);

            var scriptEndTagIndex = script.LastIndexOf(ScriptEndTag);
            script = script.Remove(scriptEndTagIndex, ScriptEndTag.Length);

            if (!string.IsNullOrWhiteSpace(bindingElementSelector))
            {
                var element = "$('" + bindingElementSelector + "')[0]";
                script = script.Replace("ko.applyBindings(viewModel);",
                                        "ko.applyBindings(viewModel, " + element + ");");
            }

            return new HtmlString(script);
        }
    }
}