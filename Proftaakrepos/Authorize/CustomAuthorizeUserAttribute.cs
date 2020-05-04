using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary.Classes;

namespace Proftaakrepos.Authorize
{
    public class UserAccessAttribute : TypeFilterAttribute
    {
        public UserAccessAttribute(string type, string pagina) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(type, pagina) };
        }
    }
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public ClaimRequirementFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_claim.Type.ToLower() == "iedereen")
            {

            }else if (_claim.Type.ToLower() == "loggedin")
            {
                if(context.HttpContext.Session.GetString("UserInfo") == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "Error401" }));
                }
            }
            else
            {
                string authCode = context.HttpContext.Session.GetString("UserInfo");
                string pagina = _claim.Value;
                string rol;
                if (authCode != null)
                {
                    List<string> rollen = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authCode}'");
                    if (rollen.Count > 0)
                    {
                        rol = rollen[0];
                        List<string[]> response = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT `{pagina.ToLower()}` FROM `Rollen` WHERE `Naam` = '{rol}'");
                        if (response.Count > 0)
                        {
                            if (response[0][0] == "True" || response[0][0] == "1") { }
                            else context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "Error401" }));
                        }
                    }
                }
                else
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "Error401" }));
                }
            }
        }
    }
}