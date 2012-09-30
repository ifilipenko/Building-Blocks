using System;
using System.Net;
using System.Text;

namespace BuildingBlocks.Web
{
    public class HttpErrorResult : ActionResult
    {
        public static HttpErrorResult InternalServerError(string message)
        {
            return new HttpErrorResult(HttpStatusCode.InternalServerError, message);
        }

        private readonly HttpStatusCode _httpStatusCode;
        private readonly string _message;

        public HttpErrorResult(HttpStatusCode httpStatusCode, string message)
        {
            _httpStatusCode = httpStatusCode;
            _message = message;
        }

        public HttpStatusCode HttpStatusCode
        {
            get { return _httpStatusCode; }
        }

        public string Message
        {
            get { return _message; }
        }

        public Encoding ContentEncoding { get; set; } 

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;

            response.StatusCode = (int) HttpStatusCode;
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            response.ContentType = "application/text";
            response.Write(Message);
        }
    }
}