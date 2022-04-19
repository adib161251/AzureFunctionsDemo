using DemoProject.DataModel;
using DemoProject.Repository.Interface;
using DemoProject.RepositoryLayer;
using DemoProject.Service;
using DemoProject.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Security.Claims;
using System.Text;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        //for Identity
        builder.Services.AddIdentity<Users, IdentityRole>()
                        .AddDefaultTokenProviders();

        //For Authentication

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        //.AddCookie(cfg => cfg.SlidingExpiration = true)

        //Adding JWT Bearer
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                //ValidAudience = "http://localhost:7071",
                //ValidIssuer = "http://localhost:5000",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"))
                
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.AddMvc(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
        });
        builder.Services.AddHttpClient();
        builder.Services.AddTransient<IFamilyCosmos, FamilyCosmos>();
        builder.Services.AddTransient<IUserCosmos, UserCosmos>();
        builder.Services.AddTransient<ISecurityService, SecurityService>();
        builder.Services.AddTransient<UserManager<Users>>();

        
        //builder.Services.AddTransient<IConfiguration, Configuration>();
        

        
    })
    .ConfigureAppConfiguration(app =>
        app.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("local.settings.json")
           .AddEnvironmentVariables())
    .Build();


//This will only work for Http Pipe Line.. Azure Function does not have HttpPipeLine
//In this case Authorize attribute will not work.. We need to build custom Authorize Attribute to validate the token
//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

////Activating Authorization and Authentication
//app.UseAuthentication();
//app.UseAuthentication();

host.Run();