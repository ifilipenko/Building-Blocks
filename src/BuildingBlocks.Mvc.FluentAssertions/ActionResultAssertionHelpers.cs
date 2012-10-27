using System.Web.Mvc;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace BuildingBlocks.Mvc.FluentAssertions
{
    public static class ActionResultAssertionHelpers
    {
        public static TResult GetModelOf<TResult>(this ActionResult actionResult)
        {
            var model = (TResult)((ViewResult)actionResult).Model;
            return model;
        }

        public static AndConstraint<TAssertions> BeViewResult<TSubject, TAssertions>(this ReferenceTypeAssertions<TSubject, TAssertions> assertions)
            where TAssertions : ReferenceTypeAssertions<TSubject, TAssertions>
        {
            return assertions.BeOfType<ViewResult>();
        }

        public static AndConstraint<TAssertions> BeViewResultWithEmptyName<TSubject, TAssertions>(this ReferenceTypeAssertions<TSubject, TAssertions> assertions)
            where TAssertions : ReferenceTypeAssertions<TSubject, TAssertions>
        {
            var result = assertions.BeOfType<ViewResult>();
            var viewResult = assertions.Subject as ViewResult;
            viewResult.ViewName.Should().BeNullOrEmpty();
            return result;
        }
    }
}
