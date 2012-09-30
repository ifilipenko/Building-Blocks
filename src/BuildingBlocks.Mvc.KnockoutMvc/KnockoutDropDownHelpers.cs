using System;
using System.Linq.Expressions;
using PerpetuumSoft.Knockout;

namespace BuildingBlocks.Mvc.KnockoutMvc
{
    public static class KnockoutDropDownHelpers
    {
        public static KnockoutBinding<TModel> OptionValue<TModel>(this KnockoutBinding<TModel> knockoutBinding, Expression<Func<TModel, object>> binding)
        {
            var item = new KnockoutBindingItem<TModel, object>
                {
                    Name = "optionsValue",
                    Expression = binding
                };
            knockoutBinding.Items.Add(item);
            return knockoutBinding;
        }
    }
}