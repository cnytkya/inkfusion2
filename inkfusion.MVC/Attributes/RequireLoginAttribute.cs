using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace inkfusion.MVC.Attributes
{
    /// <summary>
    /// Custom authorization attribute that checks if user is logged in via session
    /// Redirects to login page if user is not authenticated
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireLoginAttribute : Attribute, IAuthorizationFilter
    {
        private const string USER_SESSION_KEY = "UserEmail";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var session = context.HttpContext.Session;
            var userEmail = session?.GetString(USER_SESSION_KEY);

            // If no user email in session, user is not logged in
            if (string.IsNullOrEmpty(userEmail))
            {
                // Store the original URL to redirect back after login
                context.HttpContext.Session?.SetString("ReturnUrl", context.HttpContext.Request.Path.ToString());

                // Redirect to login page
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Login" },
                        { "action", "Login" },
                        { "area", "" }
                    });
            }
        }
    }
}
