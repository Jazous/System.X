namespace System.Web.Mvc
{
    public sealed class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var acceptEncoding = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding))
            {
                acceptEncoding = acceptEncoding.ToLower();
                if (acceptEncoding.Contains("gzip"))
                {
                    var response = filterContext.HttpContext.Response;
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new IO.Compression.GZipStream(response.Filter, IO.Compression.CompressionLevel.Fastest, false);
                }
                else if (acceptEncoding.Contains("deflate"))
                {
                    var response = filterContext.HttpContext.Response;
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new IO.Compression.DeflateStream(response.Filter, IO.Compression.CompressionLevel.Fastest);
                }
            }
        }
    }
}
