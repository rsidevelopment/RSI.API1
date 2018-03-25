using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Legacy.Services.Data;
using Microsoft.EntityFrameworkCore;
using Legacy.Services.Interfaces;
using Legacy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddDbContext<LegacyDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LegacyConnection"), o=> o.UseRowNumberForPaging()));
            
            services.AddTransient<IMemberService, MemberServices>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<IPackageService, PackageService>();
            services.AddTransient<ICountriesStatesService, CountriesStatesService>();
            services.AddTransient<IUnitService, UnitService>();
            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<IReservationService, ReservationService>();

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddApiExplorer();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.LegacyAudienceValidation = true;
                    //options.Authority = "https://authorize.accessrsi.com";
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Condo Engine API v1");
            });
        }
    }
}
