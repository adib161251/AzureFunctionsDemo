using DemoProject.BodyModel;
using DemoProject.DataModel;
using DemoProject.Service.Interface;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Worker.Http;
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
        private static readonly string privateKey = "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr";
        public async Task<string> GenerateJWTToken(UsersViewModel userData)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

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


        public async Task<bool> VerifyJWTTokenV2(HttpRequestData request)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(privateKey);

                if (!request.Headers.Contains("Authorization"))
                {
                    return false;
                }
                var authorizationHeader = request.Headers.ToString().Split("Authorization:")[1].Trim();

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return false;
                }

                if (authorizationHeader.StartsWith("Bearer"))
                {
                    authorizationHeader = authorizationHeader.Substring(7);
                }

                var token = new JwtSecurityToken(authorizationHeader);


                tokenHandler.ValidateToken(authorizationHeader, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = jwtToken.Claims.First(x => x.Type == "id").Value;

                // attach account to context on successful jwt validation
                

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //this algo is not working
        public async Task<bool> VerifyJWTToken(HttpRequestData request)
        {
            if (!request.Headers.Contains("Authorization"))
            {
                return false;
            }
            var authorizationHeader = request.Headers.ToString().Split("Authorization:")[1].Trim();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return false;
            }

            IDictionary<string, object>? claims = null;
            try
            {
                if(authorizationHeader.StartsWith("Bearer"))
                {
                    authorizationHeader = authorizationHeader.Substring(7);
                }
                
                //claims = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm()).WithSecret(privateKey).MustVerifySignature().Decode<IDictionary<string, object>>(authorizationHeader);
                claims = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm()).WithSecret(privateKey).MustVerifySignature().Decode<IDictionary<string, object>>(authorizationHeader);
            }
            catch(Exception ex)
            {
                return false;
            }

            if (!claims.ContainsKey("username"))
            {
                return false;
            }
            
            var Username = Convert.ToString(claims["username"]);
            var Role = Convert.ToString(claims["role"]);

            return await Task.FromResult(true);
        }

        public async Task<string> HashPassword(Users requestData)
        {
            var options = new PasswordHasherOptions();
            options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;

            var _hasher = new PasswordHasher<Users>();

            return await Task.FromResult(_hasher.HashPassword(requestData, requestData.Password));
        }

        public async Task<PasswordVerificationResult> VerifyHashPassword(Users requestData, List<Users> rawUserData)
        {
            var options = new PasswordHasherOptions();
            options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;

            var _hasher = new PasswordHasher<Users>();

            return await Task.FromResult(_hasher.VerifyHashedPassword(requestData, rawUserData[0].Password, requestData.Password));
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
