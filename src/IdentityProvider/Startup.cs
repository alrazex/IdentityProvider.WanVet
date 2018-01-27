using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityProvider.Data;
using IdentityProvider.Models;
using IdentityProvider.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityProvider
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            _environment = env;

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services) 
        {
            var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "localhost.pfx"), "local");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddTransient<SeedDbData>();

            services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();

            services.AddTransient<IEmailSender, AuthMessageSender>();

            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddIdentityServer()
                .AddSigningCredential(cert)
                .AddInMemoryScopes(Config.GetScopes())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, SeedDbData seedData)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                // Does not work with HTTPS
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // Ensure that we created the database 
                try
                {
                    using (
                        var serviceScope =
                            app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            await seedData.EnsureSeedDataAsync();

        }
    }
}
