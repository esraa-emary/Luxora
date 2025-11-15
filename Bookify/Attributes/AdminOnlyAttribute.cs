using Bookify.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bookify.Attributes
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!SessionHelper.IsLoggedIn(context.HttpContext.Session))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (!SessionHelper.IsAdmin(context.HttpContext.Session))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }
            
            base.OnActionExecuting(context);
        }
    }
}




