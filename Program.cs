using System;
using System.IO;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
      
      var authenticationProviderKey = "TestKey";
      // Action<IdentityServerAuthenticationOptions> options = o =>
      // {
      //   o.Authority = "https://whereyouridentityserverlives.com";
      //   o.ApiName = "api";
      //   o.SupportedTokens = SupportedTokens.Both;
      //   o.ApiSecret = "secret";
      // };
      
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
           s.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
             .AddIdentityServerAuthentication(authenticationProviderKey, options =>
             {
               // base-address of your identityserver
               options.Authority = "https://localhost:5001";
               // name of the API resource
               options.ApiName = "api1";
               options.ApiSecret = "secret";
             });
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
