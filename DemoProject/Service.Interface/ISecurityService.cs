using DemoProject.BodyModel;
using DemoProject.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Worker.Http;
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
        Task<bool> VerifyJWTToken(HttpRequestData userData);
        Task<bool> VerifyJWTTokenV2(HttpRequestData userData);
        Task<string> HashPassword(Users requestData);
        Task<PasswordVerificationResult> VerifyHashPassword(Users requestData, List<Users> rawUserData);
    }
}
