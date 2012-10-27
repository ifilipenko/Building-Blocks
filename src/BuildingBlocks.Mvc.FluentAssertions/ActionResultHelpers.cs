using System.Web.Mvc;

namespace BuildingBlocks.Mvc.FluentAssertions
{
    public static class ActionResultHelpers
    {
        public static TResult GetModelOf<TResult>(this ActionResult actionResult)
        {
            var model = (TResult) ((ViewResult) actionResult).Model;
            return model;
        }
    }
}