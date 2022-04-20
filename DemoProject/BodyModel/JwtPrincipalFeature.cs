using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.BodyModel
{
    public class JwtPrincipalFeature
    {
        public ClaimsPrincipal? Principal { get;}
        public string? AccessToken { get; set; }

        public JwtPrincipalFeature(ClaimsPrincipal principal, string token)
        {
            this.Principal = principal;
            this.AccessToken = token; 
        }
    }


    public class JwtProperty
    {
        public ClaimsPrincipal? Principal { get; set; }
        public string? JwtName { get; set; }
    }
}
