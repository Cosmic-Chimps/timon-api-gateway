using System.Collections.Generic;
using TimonApiGateway.Config;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace TimonApiGateway
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      var identityServerConfig = Configuration.GetSection(nameof(IdentityServerOptions));

      var identityServerOptions = new IdentityServerOptions();
      identityServerConfig.Bind(identityServerOptions);

      var authenticationProviderKey = identityServerOptions.AuthenticationProviderKey;

      services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
          .AddIdentityServerAuthentication(authenticationProviderKey, options =>
          {
                  // base-address of your identityserver
                  options.Authority = identityServerOptions.Authority;
                  // name of the API resource
                  options.ApiName = identityServerOptions.ApiName;
            options.ApiSecret = identityServerOptions.ApiSecret;
          });
      // services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
      //     .AddJwtBearer(authenticationProviderKey, options =>
      //     {
      //         options.Authority = identityServerOptions.Authority;
      //         options.Audience = identityServerOptions.ApiName;
      //     });

      // #if DEBUG
      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
                  builder => builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
      });
      // #else
      //             var allowedDomains = Configuration["AllowedDomains"];
      //             services.AddCors(options =>
      //             {
      //                 options.AddPolicy("CorsPolicy",
      //                     builder => builder.WithOrigins("allowedDomains")
      //                         .AllowAnyMethod()
      //                         .AllowAnyHeader()
      //                         .AllowCredentials());
      //             });
      // #endif

      services.AddOcelot();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app,
                                IWebHostEnvironment env)
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

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseCors("CorsPolicy");
      await app.UseOcelot(conf);
    }
  }
}
