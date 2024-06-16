using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace SmallBusiness.Attributes
{
    public class AuthorizeUserTypeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] allowedTypes;

        public AuthorizeUserTypeAttribute(params string[] types)
        {
            this.allowedTypes = types;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userType = context.HttpContext.Session.GetString("type");

            if (userType == null || !allowedTypes.Contains(userType, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("Unauthorized", "Home", null);
            }
        }
    }
}