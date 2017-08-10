using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using TodoAPI.Context;
using IdentityServer4.AccessTokenValidation;

namespace TodoAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<TodoContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //enable cors from anywhere
            services.AddCors(options =>
            {
                options.AddPolicy("corsGlobalPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

            });

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseCors("corsGlobalPolicy");

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000",
                RequireHttpsMetadata = false,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                ApiSecret = "dataSecret",
                ApiName = "data",
                //AllowedScopes = new List<string> { "data", "datascope" },
                SupportedTokens = SupportedTokens.Both,
            });

            app.UseMvc();
        }
    }
}
