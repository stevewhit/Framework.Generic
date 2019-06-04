using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Framework.Generic.Logging;

namespace Framework.Generic.MVC.Attributes
{
    [ExcludeFromCodeCoverage]
    public class LogErrorsAttribute : HandleErrorAttribute
    {
        private readonly ILog _log;

        public LogErrorsAttribute(ILog log)
        {
            _log = log;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            _log.Error("Unhandled error", filterContext.Exception);
        }
    }
}