using DemoProject.Repository.Interface;
using DemoProject.RepositoryLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.Services.AddHttpClient();
        builder.Services.AddTransient<IFamilyCosmos, FamilyCosmos>();
        builder.Services.AddTransient<IUserCosmos, UserCosmos>();
    })
    .ConfigureAppConfiguration(app =>
        app.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("local.settings.json")
           .AddEnvironmentVariables())
    .Build();

host.Run();