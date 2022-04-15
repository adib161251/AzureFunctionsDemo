using DemoProject.Repository.Interface;
using DemoProject.RepositoryLayer;
using DemoProject.Service;
using DemoProject.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;



var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })

        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = "http://localhost:7071",
                ValidIssuer = "http://localhost:5000",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"))
            };
        });


        builder.Services.AddHttpClient();
        builder.Services.AddTransient<IFamilyCosmos, FamilyCosmos>();
        builder.Services.AddTransient<IUserCosmos, UserCosmos>();
        builder.Services.AddTransient<ISecurityService, SecurityService>();
        //builder.Services.AddTransient<IConfiguration, Configuration>();
        builder.Services.AddAuthorization();
    })
    .ConfigureAppConfiguration(app =>
        app.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("local.settings.json")
           .AddEnvironmentVariables())
    .Build();

host.Run();