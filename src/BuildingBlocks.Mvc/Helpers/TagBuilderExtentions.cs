using System.Diagnostics;
using System.Web.Mvc;

namespace BuildingBlocks.Mvc.Helpers
{
    public static class TagBuilderExtentions
    {
        public static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
        {
            Debug.Assert(tagBuilder != null);
            return new MvcHtmlString(tagBuilder.ToString(renderMode));
        }
    }
}