using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tangehrine.Common.Identity;
using Tangehrine.DataLayer;
using Tangehrine.DataLayer.DbContext;
using Tangehrine.ServiceLayer.Implementation;
using Tangehrine.ServiceLayer.Implementation.Admin;
using Tangehrine.ServiceLayer.Implementation.Admin.Master;
using Tangehrine.ServiceLayer.Implementation.Common;
using Tangehrine.ServiceLayer.Interface;
using Tangehrine.ServiceLayer.Interface.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;
using Tangehrine.ServiceLayer.Mapper;
using Tangehrine.ServiceLayer.Utility;

namespace Tangehrine.WebLayer
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
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddCors();
            services.AddControllersWithViews();

            //services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);
            services.AddDbContext<TangehrineDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.ini
            services.AddIdentity<ApplicationUser, Role>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<TangehrineDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<RoleManager<Role>>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ClaimsPrincipalFactory>();
            //services.AddTransient<IEmailSender, EmailService>();

            #region Identity Configuration
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;


            });

            //Seting the Account Login page
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);    
                options.LoginPath = "/Identity/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Identity/Account/Login
                options.LogoutPath = "/Identity/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });
            //Seting the Post Configure
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
            {
                //configure your other properties
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Identity/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Identity/Account/Login
                options.LogoutPath = "/Identity/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });
            #endregion

            services.AddAuthentication
            (CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();

            #region Dependency Injection        


            services.AddScoped<ICommonService, CommonRepository>();
            services.AddScoped<IPatientMasterService,PatientMasterService>();
            services.AddScoped<IErrorLogService, ErrorLogRepository>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<ILabReportService, LabReportService>();
            services.AddScoped<ILetterFormatService, LetterFormatService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IRelationshipService, RelationshipService>();
            //services.AddScoped<IUserService, UserService>();
            // services.AddScoped<ICommonService, CommonRepository>();
            services.AddScoped<IMedicineService, MedicineService>();
            services.AddScoped<IMasterService, MasterService>();
            services.AddScoped<IUserMasterService, UserMasterService>();
            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IPatientVisitService, PatientVisitService>();
            services.AddScoped<IPreVisitNoteService, PreVisitNoteService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddScoped<IIMEService, IMEService>();
            services.AddScoped<IPatientLabReportService, PatientLabReportService>();
            services.AddScoped<IVitalService, VitalService>();
            services.AddScoped<IPatientMedicineService, PatientMedicineService>();
            services.AddScoped<IPatientStatementTrendsService, PatientStatementTrendsService>();
            services.AddScoped<IPatientCheckupRecordService, PatientCheckupRecordService>();
            services.AddScoped<ICalendarService, GoogleCalendarService>(); 
            services.AddScoped<IToDoListService,ToDoListService>();
            services.AddScoped<IPatientNoteService, PatientNoteService>();
            #endregion

            services.AddMvc();
            /**Add Automapper**/
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession(opts =>
            {
                opts.Cookie.HttpOnly = true;
                opts.Cookie.IsEssential = true; // make the session cookie Essential
                opts.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            //set login as default page
            services.AddMvc()
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddSessionStateTempDataProvider();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            //Notificatiion settings
            services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });

            #region Configure App Settings
            /**Email Settings**/
            services.Configure<EmailSettingsGmail>(Configuration.GetSection("EmailSettingsGmail"));
            /**Settings**/
            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager)
        {
            app.UseDeveloperExceptionPage();

            app.UseDeveloperExceptionPage();
            //if (env.IsDevelopment() || env.IsStaging())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseDatabaseErrorPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    IServiceProvider services = context.RequestServices;
                    IEmailSender emailSender = services.GetRequiredService<IEmailSender>();

                });
            });

            app.UseHttpsRedirection();
            // Shows UseCors with CorsPolicyBuilder.
            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            app.UseStaticFiles();
        
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseNotyf();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapAreaControllerRoute(
                   name: "Admin",
                   areaName: "Admin",
                   pattern: "Admin/{controller=Patient}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                   name: "Receptionist",
                   areaName: "Receptionist",
                   pattern: "Receptionist/{controller=VisitProfile}/{action=Index}/{id?}");


                endpoints.MapAreaControllerRoute(
                   name: "Common",
                   areaName: "Common",
                   pattern: "Common/{controller=Common}/{action=Index}/{id?}");
              

                //endpoints.MapAreaControllerRoute(
                //   name: "Client",
                //   areaName: "Client",
                //   pattern: "Client/{controller=ClientDashBoard}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

            });

            IdentityDataInitializer.SeedData(userManager, roleManager);

            //RotativaConfiguration.Setup(env);
        }
    }
}
