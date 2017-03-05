using DotNetLive.Framework.WebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetLive.Framework.WebApi.WebFramework.Filters
{

    public class GlobalExceptionAttribute : ExceptionFilterAttribute, IExceptionFilter
    {
        //https://weblog.west-wind.com/posts/2016/oct/16/error-handling-and-exceptionfilter-dependency-injection-for-aspnet-core-apis
        public override void OnException(ExceptionContext context)
        {
            DbTransactionHelper.RollbackTransaction(context.HttpContext.RequestServices);

            ApiError apiError = null;
            if (context.Exception is ApiException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as ApiException;
                context.Exception = null;
                apiError = new ApiError(ex.Message);
                apiError.errors = ex.Errors;

                context.HttpContext.Response.StatusCode = ex.StatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                context.HttpContext.Response.StatusCode = 401;

                // handle logging here
            }
            else
            {
                // Unhandled errors
#if !DEBUG
                var msg = "An unhandled error occurred.";                
                string stack = null;
#else
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
#endif

                apiError = new ApiError(msg);
                apiError.detail = stack;

                context.HttpContext.Response.StatusCode = 500;

                // handle logging here
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }

    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public Dictionary<string, Exception> Errors { get; set; }

        public ApiException(string message,
                            int statusCode = 500,
                            Dictionary<string, Exception> errors = null) :
            base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }

        public ApiException(Exception ex, int statusCode = 500) : base(ex.Message)
        {
            StatusCode = statusCode;
        }
    }

    public class ApiError
    {
        public string message { get; set; }
        public bool isError { get; set; }
        public string detail { get; set; }
        public Dictionary<string, Exception> errors { get; set; }

        public ApiError(string message)
        {
            this.message = message;
            isError = true;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            this.isError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                message = "Please correct the specified errors and try again.";
                errors = modelState.SelectMany(m => m.Value.Errors).ToDictionary(item => item.ErrorMessage, item => item.Exception);
                //errors = modelState.SelectMany(m => m.Value.Errors.Select( me => new KeyValuePair<string,string>( m.Key,me.ErrorMessage) ));
                //errors = modelState.SelectMany(m => m.Value.Errors.Select(me => new ModelError { FieldName = m.Key, ErrorMessage = me.ErrorMessage }));
            }
        }
    }
}
