using DotNetLive.Framework.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetLive.Framework.WebApi.WebFramework.Filters
{
    public class GlobalDbTransactionAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            DbTransactionHelper.CommitTransaction(context.HttpContext.RequestServices);
        }
    }
}
