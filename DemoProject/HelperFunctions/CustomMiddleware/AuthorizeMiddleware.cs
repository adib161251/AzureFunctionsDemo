using DemoProject.BodyModel;
using DemoProject.HelperFunctions.Attributes;
using DemoProject.HelperFunctions.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.HelperFunctions.CustomMiddleware
{
    public class AuthorizeMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var principleFeature = context.Features.Get<JwtPrincipalFeature>();

            var targertedMethod = context.GetTargetFunctionMethod();

            var acceptedRoles = GetAcceptedRoles(targertedMethod);

            if(acceptedRoles != String.Empty)
            {
                var authorized = AuthorizePrinciple(context, principleFeature.Principal, acceptedRoles);

                if (!authorized)
                {
                    context.SetHttpResponseStatusCode(HttpStatusCode.Forbidden);
                    return;
                }
            }

            await next(context);
        }

        private static bool AuthorizePrinciple(FunctionContext context, ClaimsPrincipal? principal, string acceptedRoles)
        {

            var claimRoles = principal?.FindAll(ClaimTypes.Role);

            var rolesAccpests = acceptedRoles.Split(',').ToArray();
            var hasAppectedRole = claimRoles.Any(x => rolesAccpests.Contains(x.Value.ToLower()));

            return hasAppectedRole;

        }

        private static string GetAcceptedRoles(System.Reflection.MethodInfo targertedMethod)
        {
            var attribute = GetCustomAttributesOnClassAndMethod<AuthorizeAttribute>(targertedMethod);

            var data = attribute.Select(x => x.Role).ToList().FirstOrDefault()?.ToLower();
            //var data = attribute.Skip(1).Select(a => a.Role).FirstOrDefault()?.ToLower();

            return (data == null?String.Empty:data);
        }


        private static List<T> GetCustomAttributesOnClassAndMethod<T>(MethodInfo targetMethod)
            where T : Attribute
        {
            var methodAttributes = targetMethod.GetCustomAttributes<T>();
            var classAttributes = targetMethod.DeclaringType.GetCustomAttributes<T>();
            return methodAttributes.Concat(classAttributes).ToList();
        }
    }
}
