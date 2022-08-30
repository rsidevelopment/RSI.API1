using AccessDevelopment.Services;
using AccessDevelopment.Services.Interfaces;
using Hangfire;
using Legacy.Services;
using Legacy.Services.Data;
using Legacy.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace RSI.API
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
            services.AddDbContext<HangFireDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HangFireConnection"), o => o.UseRowNumberForPaging()));
            services.AddDbContext<LegacyDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LegacyConnection"), o=> o.UseRowNumberForPaging()));
            services.AddDbContext<RSIDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), o => o.UseRowNumberForPaging()));

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("HangFireConnection")));

            services.AddTransient<IMemberService, MemberServices>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<IPackageService, PackageService>();
            services.AddTransient<IGeographyService, GeographyService>();
            services.AddTransient<IUnitService, UnitService>();
            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<IHangFireService, HangFireService>();
            services.AddTransient<IRCIService, RCIService>();
            services.AddTransient<IAccessDevelopmentService, AccessDevelopmentService>();

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddApiExplorer();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.LegacyAudienceValidation = true;
                    options.Authority = "https://authorize.accessrsi.com";
                    //options.Authority = "http://localhost:60195";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api1";
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Condo Engine API", Version = "v1" });
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            /*.AddIdentityServerAuthentication(options =>
            {
                options.Authority = "https://authorize.accessrsi.com";
                options.ApiName = "api1";
            });*/
            /*app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "https://authorize.accessrsi.com",
                 scop
                //Authority = "http://localhost:58580",
                AllowedScopes = new[] { "api1" },
                //ScopeName = ,

                RequireHttpsMetadata = false
            });*/

            GlobalConfiguration.Configuration
                .UseActivator(new HangfireActivator(serviceProvider));

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                Queues = new[] { "rsi_api" }
            });
            app.UseHangfireDashboard();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Condo Engine API v1");
            });
        }
    }
}
