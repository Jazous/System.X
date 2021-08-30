namespace System.Web.Mvc
{
    public sealed class JsonAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is JsonResult)
            {
                JsonResult result = (JsonResult)filterContext.Result;
            }
            base.OnResultExecuting(filterContext);
        }
    }
}