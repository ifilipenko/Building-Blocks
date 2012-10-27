using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BuildingBlocks.Mvc.Helpers
{
    public static class SelectListHelper
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> enumerable, Func<T, string> titleGetter, Func<T, string> valueGetter = null, Func<T, bool> selectedCondition = null)
        {
            if (enumerable == null)
                return Enumerable.Empty<SelectListItem>();

            if (valueGetter == null)
            {
                valueGetter = titleGetter;
            }

            return from item in enumerable 
                   let selected = selectedCondition != null && selectedCondition(item) 
                   select new SelectListItem
                              {
                                  Selected = selected,
                                  Text = titleGetter(item),
                                  Value = valueGetter(item)
                              };
        }
    }
}