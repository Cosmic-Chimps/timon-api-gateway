using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;

namespace ApiGateway
{
  class Program
  {
    static void Main(string[] args)
    {
      var conf = new OcelotPipelineConfiguration()
      {
        PreErrorResponderMiddleware = async (ctx, next) =>
        {
          if (ctx.Request.Path.Equals(new PathString("/")))
          {
            await ctx.Response.WriteAsync("ok");
          }
          else
          {
            await next.Invoke();
          }
        }
      };
      
      new WebHostBuilder()
         .UseKestrel()
         .UseContentRoot(Directory.GetCurrentDirectory())
         .ConfigureAppConfiguration((hostingContext, config) =>
         {
           config
                     .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                     .AddJsonFile("appsettings.json", true, true)
                     .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                     .AddJsonFile("ocelot.json")
                     .AddEnvironmentVariables();
         })
         .ConfigureServices(s =>
         {
           s.AddOcelot();
         })
         .ConfigureLogging((hostingContext, logging) =>
         {
           //add your logging
         })
         .UseIISIntegration()
         .Configure(app =>
         {
           app.UseOcelot(conf).Wait();
         })
         .Build()
         .Run();
    }
  }
}
