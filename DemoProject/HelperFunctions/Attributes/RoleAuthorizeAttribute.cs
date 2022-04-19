using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.HelperFunctions.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RoleAuthorizeAttribute : Attribute
    {
        public string something { get; set; } = "Hello";
    }
}
