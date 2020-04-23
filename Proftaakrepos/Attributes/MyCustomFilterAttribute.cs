using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proftaakrepos.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class MyCustomFilterAttribute : ActionFilterAttribute
    {

        private MyCustomFilterMode _Mode = MyCustomFilterMode.Respect;        // this is the default, so don't always have to specify
        private string role;
        private string authCode;

        public MyCustomFilterAttribute(string role, string authCode)
        {
            this.role = role;
            this.authCode = authCode;
        }
        public MyCustomFilterAttribute(MyCustomFilterMode mode)
        {
            _Mode = mode;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_Mode == MyCustomFilterMode.Ignore)
            {
                return;
            }
            string roleFromUser = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode`")[0];
            if (roleFromUser == role.ToLower())
            {

            }
        }

    }

    public enum MyCustomFilterMode
    {
        Ignore = 0,
        Respect = 1
    }
}
