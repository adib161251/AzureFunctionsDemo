using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.HelperFunctions.Attributes
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public  void OnAuthorization(AuthorizationFilterContext context)
        {
            throw new NotImplementedException();
        }

    }
}
