using System;
using coreproject.Model;
using coreproject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace coreproject.Attributes
{
    public class AuthorizeAttribute: Attribute, IAuthorizationFilter
{
     public string _role { get; set; }
     public AuthorizeAttribute(string role = "")
     {
         _role = role;
     }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var account = (Account)context.HttpContext.Items["Account"];
        if(account != null && account.AccessTokenExpiryDate < DateTime.Now){
             context.Result = new JsonResult(new { message = "Access Token expired" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
        if (account == null || (account.Role != _role && _role != ""))
        {
            // not logged in or role not authorized
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }        
    }
}

}