using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace BuildingBlocks.Mvc.FluentAssertions
{
    public static class ActionResultAssertionHelpers
    {
        public static AndConstraint<ObjectAssertions> BeRedirectToAction(this ObjectAssertions assertions, string actionName, string controllerName = null, string area = null)
        {
            var result = assertions.BeOfType<RedirectToRouteResult>();
            var actionResult = (RedirectToRouteResult)assertions.Subject;
            actionResult.RouteValues.Should().ContainKey("action");
            actionResult.RouteValues["action"].Should().Be(actionName);
            if (controllerName != null)
            {
                actionResult.RouteValues.Should().ContainKey("controller");
                actionResult.RouteValues["controller"].Should().Be(controllerName);
            }
            if (area != null)
            {
                actionResult.RouteValues.Should().ContainKey("area");
                actionResult.RouteValues["area"].Should().Be(area);
            }
            return result;
        }

        public static AndConstraint<ObjectAssertions> BeRedirectToResult(this ObjectAssertions assertions, string redirect)
        {
            var result = assertions.BeOfType<RedirectResult>();
            var actionResult = (RedirectResult)assertions.Subject;
            actionResult.Url.Should().Be(redirect);
            return result;
        }

        public static AndConstraint<ObjectAssertions> BeView(this ObjectAssertions assertions, Expression<Func<ViewResult, bool>> predicate = null)
        {
            var result = assertions.BeOfType<ViewResult>();
            AssertViewResult(assertions, predicate);
            return result;
        }

        public static AndConstraint<ObjectAssertions> BeViewWithName(this ObjectAssertions assertions, string viewName)
        {
            return assertions.BeView(vr => vr.ViewName == viewName);
        }

        public static AndConstraint<ObjectAssertions> BeViewWithDefaultName(this ObjectAssertions assertions)
        {
            return assertions.BeView(vr => string.IsNullOrWhiteSpace(vr.ViewName));
        }

        public static AndConstraint<ObjectAssertions> AssertViewDataType(this ObjectAssertions assertions, Func<ViewDataDictionary, bool> viewDataAssert)
        {
            var result = assertions.BeOfType<ViewResult>();
            var actionResult = (ViewResult)assertions.Subject;
            viewDataAssert(actionResult.ViewData).Should().BeTrue();
            return result;
        }

        public static AndConstraint<ObjectAssertions> HaveModelTypeOf<T>(this ObjectAssertions assertions)
        {
            var result = assertions.BeOfType<ViewResult>();
            var actionResult = (ViewResult)assertions.Subject;
            actionResult.Model.Should().BeOfType<T>();
            return result;
        }

        public static object Model(this ObjectAssertions assertions)
        {
            assertions.BeOfType<ViewResult>();
            var actionResult = (ViewResult) assertions.Subject;
            return actionResult.Model;
        }

        private static void AssertViewResult(ObjectAssertions assertions, Expression<Func<ViewResult, bool>> predicate)
        {
            var actionResult = (ViewResult) assertions.Subject;
            if (predicate != null)
            {
                actionResult.Should().Match(predicate);
            }
        }
    }
}
