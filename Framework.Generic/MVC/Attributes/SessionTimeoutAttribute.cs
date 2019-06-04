using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using Framework.Generic.Logging;

namespace Framework.Generic.MVC.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        private readonly ILog _log;
        private readonly bool _forceSessionTimeout = false;
        private readonly string _sessionTimeoutRedirectActionName;
        private readonly string _sessionTimeoutRedirectControllerName;

        public SessionTimeoutAttribute(string sessionTimeoutRedirectActionName, bool forceSessionTimeout = false)
        {
            _forceSessionTimeout = forceSessionTimeout;
            _sessionTimeoutRedirectActionName = sessionTimeoutRedirectActionName;
        }

        public SessionTimeoutAttribute(ILog log, string sessionTimeoutRedirectActionName, bool forceSessionTimeout = false)
            : this(sessionTimeoutRedirectActionName, forceSessionTimeout)
        {
            _log = log;
        } 

        public SessionTimeoutAttribute(string sessionTimeoutRedirectActionName, string sessionTimeoutRedirectControllerName, bool forceSessionTimeout = false)
            : this(sessionTimeoutRedirectActionName, forceSessionTimeout)
        {
            _sessionTimeoutRedirectControllerName = sessionTimeoutRedirectControllerName;
        }

        public SessionTimeoutAttribute(ILog log, string sessionTimeoutRedirectActionName, string sessionTimeoutRedirectControllerName, bool forceSessionTimeout = false)
            : this(sessionTimeoutRedirectActionName, sessionTimeoutRedirectControllerName, forceSessionTimeout)
        {
            _log = log;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {           
            //Check if the session has been cleared due to a timeout
            var ctx = HttpContext.Current;
            if (ctx.Session != null)
            {
                if (ctx.Session.IsNewSession)
                {
                    string sessionCookie = ctx.Request.Headers["Cookie"];

                    if (_forceSessionTimeout || (sessionCookie != null && sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        ctx.Request.Cookies.Clear();

                        var controller = filterContext.HttpContext.Request.RequestContext.RouteData.Values["controller"];
                        var action = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"];
                        var url = new UrlHelper(filterContext.RequestContext);

                        // Re-direct the user to an appropriate page.
                        var sessionClearedUrl = url.Action(_sessionTimeoutRedirectActionName ?? "Logout",
                                                           _sessionTimeoutRedirectControllerName ?? controller);
                        filterContext.Result = new RedirectResult(sessionClearedUrl);

                        if (_log != null)
                            _log.Info("Session and cookies cleared.");
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
