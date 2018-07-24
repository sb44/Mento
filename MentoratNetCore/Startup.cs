using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MentoratNetCore.Data;
using MentoratNetCore.Models;
using MentoratNetCore.Services;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace MentoratNetCore
{
    public class Startup
    {
        public static string WebRootPath { get; private set; }
        public static string ConnectionString { get; private set; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            WebRootPath = env.WebRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Startup.ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            // SB: Service enlevé et remplacer par code suivant avec "ApplicationRole"
            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // If the LoginPath isn't set, ASP.NET Core defaults 
                // the path to /Account/Login.
                options.LoginPath = "/Account/Login";
                // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
                // the path to /Account/AccessDenied.
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();


            // services.AddDistributedMemoryCache();

            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-2.1
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30); // TODO : EFFACER CECI OU METTRE PLUS LONG... LE TEMPS EST 20 MINUTES PAR DÉFAUT!
                options.Cookie.HttpOnly = true;
            });


            services
                .AddMvc()
                // Maintain property names during serialization. See:
                // https://github.com/aspnet/Announcements/issues/194
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); ;

           // //  Usage de policy : https://gooroo.io/GoorooTHINK/Article/17333/Custom-user-roles-and-rolebased-authorization-in-ASPNET-core/28352#.W1XsmNVKjRZ
           // services.AddAuthorization(options =>
           // {
           //     options.AddPolicy("RequireSasha", policy => policy.RequireUserName("sasha")); // [Authorize(Policy = "RequireSasha")]
           //     options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin")); // [Authorize(Policy = "RequireAdminRole")]
           // });


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add Kendo UI services to the services container
            services.AddKendo();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Accueil/Error");
                app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
            }

            app.UseStaticFiles();

            // Identity is enabled for the application by calling UseAuthentication in the Configure method.
            // UseAuthentication adds authentication middleware to the request pipeline.
            app.UseAuthentication();

            // app.UseResponseCaching(); // https://stackoverflow.com/questions/27304210/how-do-i-apply-the-outputcache-attribute-on-a-method-in-a-vnext-project
            app.UseSession(); // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-2.1

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Accueil}/{action=Index}/{id?}");
            });

        }
    }
}
