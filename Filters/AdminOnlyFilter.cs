using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Filters;

public class AdminOnlyFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated ||
            !context.HttpContext.User.IsInRole("Администратор"))
        {
            context.Result = new ForbidResult();
        }
    }
}