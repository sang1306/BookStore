using BookStore.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.Filters
{
    public class AuthenticationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // return to login page when doen't have user session
            var httpContext = context.HttpContext;

            var userSession = UserSessionManager.GetUserInfo(httpContext);
            if (userSession == null)
            {
                context.Result = new RedirectToActionResult("index", "authentication", null);
                return;
            }
        }
    }
}
