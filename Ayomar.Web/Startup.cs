using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ayomar.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Ayomar.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver(); });
            services.AddSession(options => options.IdleTimeout = TimeSpan.FromHours(5));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region 接口注入
            services.AddTransient<Service.ISysAdminService, Service.SysAdminService>();
            services.AddTransient<Service.ISysModuleService, Service.SysModuleService>();
            services.AddTransient<Service.ISysSystemService, Service.SysSystemService>();
            services.AddTransient<Service.ISysPermissionService, Service.SysPermissionService>();
            services.AddTransient<Service.ISysDepartmentService, Service.SysDepartmentService>();
            services.AddTransient<Service.ISysRoleServcie, Service.SysRoleService>();
            services.AddTransient<Service.ISysRoleAssignmentServcie, Service.SysRoleAssignmentServcie>();
            services.AddTransient<Service.ISysRolePermissionService, Service.SysRolePermissionService>();
            services.AddTransient<Service.ISysPendingTrialService, Service.SysPendingTrialService>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();
            app.UseMvc();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                #region 管理后台
                routes.MapRoute(
                   name: "areaRoute",
                   template: "{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                   name: "areaRouteApi",
                   template: "{area:exists}/{controller}/{action=Index}/{id?}");
                #endregion

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
