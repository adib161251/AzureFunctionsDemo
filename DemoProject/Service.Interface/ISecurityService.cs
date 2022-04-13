using DemoProject.BodyModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service.Interface
{
    public interface ISecurityService
    {
        Task<string> GenerateJWTToken(UsersViewModel userData);
    }
}
