using BLL;
using BLL.Data;
using BLL.Data.Repositories;
using BLL.Services;
using DrinkManagerWeb.Extensions;
using DrinkManagerWeb.Middlewares;
using DrinkManagerWeb.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace DrinkManagerWeb
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
            services.AddDbContext<DrinkAppContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<AppUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DrinkAppContext>();

            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("en-GB");
                options.AddSupportedCultures("en-GB", "pl-PL");
                options.AddSupportedUICultures("en-GB", "pl-PL");
                options.FallBackToParentUICultures = true;

                options
                    .RequestCultureProviders
                    .Remove(typeof(AcceptLanguageHeaderRequestCultureProvider));
            });

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                    {
                        facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                        facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                    })
                .AddGoogle(options =>
                    {
                        IConfigurationSection googleAuthNSection =
                            Configuration.GetSection("Authentication:Google");

                        options.ClientId = googleAuthNSection["ClientId"];
                        options.ClientSecret = googleAuthNSection["ClientSecret"];
                    });

            services.AddScoped<IDrinkRepository, DrinkRepository>();
            services.AddScoped<IDrinkSearchService, DrinkSearchService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IReportingModuleService, ReportingModuleService>();
            services.AddScoped<IFavouriteRepository, FavouriteRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();

            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddSingleton<BackgroundJobScheduler>();
            services.AddHostedService(provider => provider.GetService<BackgroundJobScheduler>());

            services
                .AddRazorPages()
                .AddViewLocalization();


            services.AddScoped<RequestLocalizationCookiesMiddleware>();
            services.AddControllersWithViews().
                AddRazorRuntimeCompilation().
                AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                });

            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (ctx, next) =>
            {
                using (LogContext.PushProperty("IPAddress", ctx.Connection.RemoteIpAddress))
                {
                    await next();
                }
            });
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseRequestLocalizationCookies();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            CreateRolesAsync(serviceProvider);
        }

        private async Task CreateRolesAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = await userManager.FindByEmailAsync(Configuration["AppSettings:AdminUserEmail"]).ConfigureAwait(false);
            if (user == null)
            {

                //initializing custom roles 
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roleNames = { "Admin", "User" };

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false);
                    if (!roleExist)
                    {
                        //create the roles and seed them to the database
                        await roleManager.CreateAsync(new IdentityRole(roleName)).ConfigureAwait(false);
                    }
                }

                //creating a power user who will maintain the app
                var powerUser = new AppUser()
                {
                    UserName = Configuration["AppSettings:AdminUserEmail"],
                    Email = Configuration["AppSettings:AdminUserEmail"],
                };

                string userPassword = Configuration["AppSettings:UserPassword"];

                var createPowerUser = await userManager.CreateAsync(powerUser, userPassword).ConfigureAwait(false);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await userManager.AddToRoleAsync(powerUser, "Admin").ConfigureAwait(false);
                }
            }
        }
    }
}