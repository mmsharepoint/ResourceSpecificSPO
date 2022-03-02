using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using System.Net;

[assembly: FunctionsStartup(typeof(ResourceSpecificSPO.Startup))]

namespace ResourceSpecificSPO
{
  public class Startup : FunctionsStartup
  {
    public override void Configure(IFunctionsHostBuilder builder)
    {
      ServicePointManager.Expect100Continue = true;
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                             | SecurityProtocolType.Tls11
                                             | SecurityProtocolType.Tls12;

      var config = builder.GetContext().Configuration;
      var appConfig = new CustomSettings();
      config.Bind(appConfig);

      builder.Services.AddSingleton(appConfig);
      // builder.Services.AddLogging();
      builder.Services.AddScoped<controller.TokenValidation>();
    }

    //public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    //{
    //  builder.ConfigurationBuilder
    //    .AddEnvironmentVariables()
    //    .Build();
    //}
    //public void ConfigureServices(IServiceCollection services)
    //{
    //  IdentityModelEventSource.ShowPII = true;
    //}
  }
}
