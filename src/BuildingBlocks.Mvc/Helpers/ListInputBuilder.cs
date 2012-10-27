using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BuildingBlocks.Mvc.Helpers
{
    internal class ListInputBuilder
    {
        private readonly HtmlHelper _htmlHelper;

        public ListInputBuilder(HtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public bool AllowMultiple { get; set; }
        public IDictionary<string, object> HtmlAttributes { get; set; }
        public IEnumerable<SelectListItem> InputList { get; set; }
        public string Name { get; set; }

        public MvcHtmlString Render()
        {
            var fullName = _htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(Name);
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Null or Empty", "name");
            }

            var usedViewData = false;
            var inputList = InputList;
            if (inputList == null)
            {
                inputList = GetInputData(fullName);
                usedViewData = true;
            }

            var defaultValue = GetDefaultValue(fullName, usedViewData);
            if (defaultValue != null)
            {
                inputList = GetListWithDefaultValue(inputList, defaultValue);
            }
            
            var listItemBuilder = new StringBuilder();
            if (inputList != null)
            {
                foreach (var item in inputList)
                {
                    listItemBuilder.AppendLine(InputItemToHtml(item, fullName));
                }
            }

            var tagBuilder = new TagBuilder("ul")
                                 {
                                     InnerHtml = listItemBuilder.ToString()
                                 };

            tagBuilder.Attributes.Add("style", "padding:0");
            tagBuilder.MergeAttributes(HtmlAttributes, true);
            tagBuilder.GenerateId(fullName);

            ModelState modelState;
            if (_htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }

            tagBuilder.MergeAttributes(_htmlHelper.GetUnobtrusiveValidationAttributes(Name));
            return tagBuilder.ToMvcHtmlString(TagRenderMode.Normal);
        }

        private IEnumerable<SelectListItem> GetInputData(string name)
        {
            object inputObject = null;
            if (_htmlHelper.ViewData != null)
            {
                inputObject = _htmlHelper.ViewData.Eval(name);
            }

            if (inputObject != null)
            {
                var inputList = inputObject as IEnumerable<SelectListItem>;
                if (inputList == null)
                {
                    throw new InvalidOperationException(
                        String.Format(
                            CultureInfo.CurrentCulture,
                            "The ViewData item that has the key '{0}' is of type '{1}' but must be of type '{2}'.",
                            name,
                            inputObject.GetType().FullName,
                            "IEnumerable<SelectListItem>"));
                }

                return inputList;
            }

            return null;
        }

        private object GetDefaultValue(string fullName, bool usedViewData)
        {
            var defaultValue = AllowMultiple
                                   ? GetModelStateValue(fullName, typeof (string[]))
                                   : GetModelStateValue(fullName, typeof (string));
            if (defaultValue == null && !usedViewData)
            {
                defaultValue = _htmlHelper.ViewData.Eval(fullName);
            }

            return defaultValue;
        }

        private object GetModelStateValue(string key, Type destinationType)
        {
            ModelState modelState;
            if (_htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState) && modelState.Value != null)
            {
                return modelState.Value.ConvertTo(destinationType, null);
            }
            return null;
        }

        private IEnumerable<SelectListItem> GetListWithDefaultValue(IEnumerable<SelectListItem> inputList, object defaultValue)
        {
            var defaultValues = AllowMultiple ? defaultValue as IEnumerable : new[] { defaultValue };
            var values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
            var selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
            var newInputList = new List<SelectListItem>();

            foreach (var item in inputList)
            {
                item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                newInputList.Add(item);
            }

            return newInputList;
        }

        private string InputItemToHtml(SelectListItem item, string name)
        {
            var listItemBuilder = new StringBuilder();
            listItemBuilder.AppendLine(RenderInputItem(item, name));
            listItemBuilder.AppendLine(CoverWithTag("span", HttpUtility.HtmlEncode(item.Text)));

            var label = CoverWithTag("label", listItemBuilder.ToString());
            return CoverWithLi(label);
        }

        private string RenderInputItem(SelectListItem item, string name)
        {
            var builder = new TagBuilder("input");
            builder.Attributes["name"] = name;
            if (AllowMultiple)
            {
                builder.Attributes["type"] = "checkbox";
            }
            else
            {
                builder.Attributes["type"] = "radio";
            }

            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            else
            {
                builder.Attributes["value"] = item.Text;
            }

            if (item.Selected)
            {
                builder.Attributes["checked"] = "checked";
            }
            return builder.ToString(TagRenderMode.Normal);
        }

        private static string CoverWithTag(string tagName, string html)
        {
            var builder = new TagBuilder(tagName) {InnerHtml = html};
            return builder.ToString(TagRenderMode.Normal);
        }

        private static string CoverWithLi(string html)
        {
            var builder = new TagBuilder("li") {InnerHtml = html};
            builder.Attributes.Add("style", "list-style-type:none;");
            return builder.ToString(TagRenderMode.Normal);
        }
    }
}