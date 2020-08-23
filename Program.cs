using System;
using System.IO;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;

namespace ApiGateway
{
  class Program
  {
    static void Main(string[] args)
    {
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .AddCommandLine(args)
        .Build();

      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
              config
                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddJsonFile("ocelot.json")
                .AddJsonFile($"configuration.{hostingContext.HostingEnvironment.EnvironmentName}.json")
                .AddEnvironmentVariables();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
              //add your logging
    #if DEBUG
                  logging.AddDebug().AddConsole();
    #endif
              })
            .UseConfiguration(configuration);
        }).Build().Run();

      // new WebHostBuilder()
      //    .UseKestrel()
      //    .UseContentRoot(Directory.GetCurrentDirectory())
      //    .ConfigureAppConfiguration((hostingContext, config) =>
      //    {
      //      config
      //                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
      //                .AddJsonFile("appsettings.json", true, true)
      //                .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
      //                .AddJsonFile("ocelot.json")
      //                .AddEnvironmentVariables();
      //    })
      //    .ConfigureServices(s =>
      //    {
      //      
      //    })
      //    .ConfigureLogging((hostingContext, logging) =>
      //    {
      //      //add your logging
      //      #if DEBUG
      //      logging.AddDebug().AddConsole();
      //      #endif
      //    })
      //    .UseIISIntegration()
      //    .Configure(app =>
      //    {
      //      app.UseOcelot(conf).Wait();
      //    })
      //    .Build()
      //    .Run();
    }
  }
}
