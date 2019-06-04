using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using Framework.Generic.Logging;

namespace Framework.Generic.MVC.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly ILog _log;
        private readonly string _unauthorizedRequestRedirectActionName;
        private readonly string _unauthorizedRequestRedirectControllerName;

        public AuthorizeRolesAttribute(string unauthorizedRequestRedirectActionName, params object[] authorizedRoles)
        {
            _unauthorizedRequestRedirectActionName = unauthorizedRequestRedirectActionName;

            if (authorizedRoles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("The roles parameter may only contain enums", "authorizedRoles");

            Roles = string.Join(",", authorizedRoles.Select(r => Enum.GetName(r.GetType(), r)));
        }
        
        public AuthorizeRolesAttribute(ILog log, string unauthorizedRequestRedirectActionName, params object[] authorizedRoles)
            : this(unauthorizedRequestRedirectActionName, authorizedRoles)
        {
            _log = log;
        }

        public AuthorizeRolesAttribute(string unauthorizedRequestRedirectActionName, string unauthorizedRequestRedirectControllerName, params object[] authorizedRoles)
            : this(unauthorizedRequestRedirectActionName, authorizedRoles)
        {
            _unauthorizedRequestRedirectControllerName = unauthorizedRequestRedirectControllerName;
        }

        public AuthorizeRolesAttribute(ILog log, string unauthorizedRequestRedirectActionName, string unauthorizedRequestRedirectControllerName, params object[] authorizedRoles)
            : this(log, unauthorizedRequestRedirectActionName, authorizedRoles)
        {
            _unauthorizedRequestRedirectControllerName = unauthorizedRequestRedirectControllerName;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var authorizedRoles = base.Roles;
            var user = filterContext.HttpContext.User;

            // Don't passively allow access if roles or user isn't provided.
            if (string.IsNullOrEmpty(authorizedRoles) || user == null)
                HandleUnauthorizedRequest(filterContext);

            // If the user is NOT in any of the authorized roles, proceed.
            if (!authorizedRoles.Split(',').Any(r => user.IsInRole(r)))
                HandleUnauthorizedRequest(filterContext);

            // Log the authorized access request.
            if (_log != null)
            {
                var controller = filterContext.HttpContext.Request.RequestContext.RouteData.Values["controller"];
                var action = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"];

                _log.Info(string.Format("Authorized page access to: {0}\\{1}", controller, action));
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var controller = filterContext.HttpContext.Request.RequestContext.RouteData.Values["controller"];
            var action = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"];
            
            // Re-direct the user to an appropriate page.
            var url = new UrlHelper(filterContext.RequestContext);
            var accessDeniedUrl = url.Action( _unauthorizedRequestRedirectActionName ?? "Unauthorized", 
                                              _unauthorizedRequestRedirectControllerName ?? controller);
            filterContext.Result = new RedirectResult(accessDeniedUrl);

            // Log the unauthorized access request.
            if (_log != null)
                _log.Warn(string.Format("Unauthorized page access to: {0}\\{1}", controller, action));

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}