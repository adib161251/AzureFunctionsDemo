using DemoProject.BodyModel;
using DemoProject.HelperFunctions.Attributes;
using DemoProject.HelperFunctions.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoProject.HelperFunctions.CustomMiddleware
{
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private static readonly string privateKey = "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr";

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var funcName = context.FunctionDefinition.Name;

            var targetMethod = context.GetTargetFunctionMethod();
            var doesHaveAuthAtr = CheckforAuthAtr(targetMethod);

            //if(!GlobalModel.UnAuthorizedFunctions.Contains(funcName))
            if(doesHaveAuthAtr)
            {
                var header = GetTokenFromHeader(context, out string token);
                if (!header)
                {
                    context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                    return;
                }

                var value = ValidateJwtToken(token);
                if(value == null)
                {
                    context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                    return;
                }

                context.Features.Set(new JwtPrincipalFeature(value.Principal, token));
            }
           
            await next(context);
        }


        private static bool GetTokenFromHeader(FunctionContext context, out string token)
        {
            token = null;
            if(!context.BindingContext.BindingData.TryGetValue("Headers", out var headersObj))
            {
                return false;
            }

            if(headersObj is not string headersStr)
            {
                return false;
            }

            var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersStr);
            var normalizedKeyHeaders = headers.ToDictionary(a=> a.Key.ToLowerInvariant(), a=> a.Value);
            
            if(!normalizedKeyHeaders.TryGetValue("authorization", out var authHeaderValue))
            {
                return false;
            }

            if(!authHeaderValue.StartsWith("Bearer", StringComparison.Ordinal))
            {
                return false;
            }

            token = authHeaderValue.Substring("Bearer ".Length).Trim();
            return true;
        }


        private static JwtProperty ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(privateKey);

            try
            {
                var principle = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var jwtName = jwtToken.Claims.FirstOrDefault(a => a.Type == "name").Value;

                return new JwtProperty { JwtName = jwtName , Principal = principle };
            }
            catch (SecurityTokenException ex)
            {
                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        private static bool CheckforAuthAtr(MethodInfo targetedMethod)
        {
            var methodAttributes = targetedMethod.GetCustomAttributes<AuthorizeAttribute>();
            var classAttributes = targetedMethod.DeclaringType.GetCustomAttributes<AuthorizeAttribute>();
            var data = methodAttributes.Concat(classAttributes).ToList();

            if(data.Count > 0) return true;
            else return false;
        }


    }
}
