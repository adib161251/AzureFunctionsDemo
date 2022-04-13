using DemoProject.BodyModel;
using DemoProject.Service.Interface;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class SecurityService : ISecurityService
    {
        public async Task<string> GenerateJWTToken(UsersViewModel userData)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"));

            var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userData.Id.ToString()),
                    new Claim(ClaimTypes.Role, (userData.AdminType == 1?"Admin":"User"))
                };


            var tokenHandler = new JwtSecurityToken(
                issuer: "http://localhost:7071",
                audience: "http://localhost:5000",
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenHandler);
            return await Task.FromResult(token);
        }


        //public CourierUsersViewModel GetToken(CourierUsersViewModel user)
        //{
        //    // authentication successful so generate jwt token
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, user.CourierUserId.ToString()),
        //             new Claim(ClaimTypes.Role, "DeliveryTiger")
        //        }),
        //        Expires = DateTime.UtcNow.AddDays(1),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    user.Token = tokenHandler.WriteToken(token);
        //    user.RefreshToken = Guid.NewGuid().ToString();
        //    // remove password before returning
        //    user.Password = null;

        //    // update Refreshtoken
        //    var entity = _sqlServerContext.CourierUsers.FirstOrDefault(item => item.CourierUserId == user.CourierUserId);
        //    entity.Refreshtoken = user.RefreshToken;
        //    entity.FirebaseToken = user.FirebaseToken;
        //    _sqlServerContext.CourierUsers.Update(entity);
        //    _sqlServerContext.SaveChanges();

        //    return user;
        //}
    }
}
