using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Proftaakrepos.Authorize
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string pagina2, string pagina) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(pagina, pagina) };
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
            string authCode = context.HttpContext.Session.GetString("UserInfo");
            string pagina = _claim.Type;
            //Rol ophalen.
            //Toegang ophalen van de pagina.
            //Access geven of Access verbieden.
        }
    }
}