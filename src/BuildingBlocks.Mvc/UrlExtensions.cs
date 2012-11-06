using System;
using System.Web.Mvc;

namespace BuildingBlocks.Mvc
{
    public static class UrlExtensions
    {
        public static string AbsoluteAction(this UrlHelper url, string action, string controller, object routeValues = null)
        {
            var request = url.RequestContext.HttpContext.Request;
            var actionUrl = url.Action(action, controller, routeValues, request.Url.Scheme);
            var relativeUri = url.ToRelativeUri(actionUrl);
            return url.ToPublicUrl(relativeUri);
        }

        public static Uri ToRelativeUri(this UrlHelper urlHelper, string absoluteUrl)
        {
            return urlHelper.BaseUri().MakeRelativeUri(new Uri(absoluteUrl));
        }

        public static Uri BaseUri(this UrlHelper urlHelper)
        {
            var request = urlHelper.RequestContext.HttpContext.Request;
            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, urlHelper.Content("~"));
            return new Uri(baseUrl);
        }

        /// <summary>
        /// This helper cut port if current request is not local
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        public static string ToPublicUrl(this UrlHelper urlHelper, Uri relativeUri)
        {
            var httpContext = urlHelper.RequestContext.HttpContext;
            var uriBuilder = new UriBuilder
                {
                    Host = httpContext.Request.Url.Host,
                    Path = "/",
                    Port = 80,
                    Scheme = "http",
                };

            if (httpContext.Request.IsLocal)
            {
                uriBuilder.Port = httpContext.Request.Url.Port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }
    }
}