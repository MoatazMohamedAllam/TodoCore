using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodoCore.Data;
using TodoCore.Models;
using TodoCore.Services;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace TodoCore
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            _environment = env;

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Adds IdentityServer
            var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "identserver.pfx"), "");

            services.AddIdentityServer()
              .AddSigningCredential(cert)
              .AddInMemoryIdentityResources(Config.GetIdentityResources())
              .AddInMemoryApiResources(Config.GetApiResources())
              .AddInMemoryClients(Config.GetClients())
              .AddAspNetIdentity<ApplicationUser>()
              .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

            services.AddCors(options =>
            {
                options.AddPolicy("corsGlobalPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //uncomment, run once after migration and database is updated
            //adds roles
            //RolesData.SeedRoles(app.ApplicationServices).Wait();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseIdentityServer();

            app.UseCors("corsGlobalPolicy");

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public static class RolesData
        {
            private static readonly string[] Roles = new string[] { "admin", "moderator", "user", "company" };

            public static async Task SeedRoles(IServiceProvider serviceProvider)
            {
                Console.WriteLine("&************************ here");
                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                    //if (dbContext.Database.GetPendingMigrations().Any())
                    //{
                    //    await dbContext.Database.MigrateAsync();

                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    Console.WriteLine("&************************ here 22222");
                    foreach (var role in Roles)
                    {
                        Console.WriteLine("&************************  " + role);
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            Console.WriteLine("&************************ added");
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }
                    // }
                }
            }
        }
    }
}

