using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Extensions;
using KnifeZ.Virgo.Core.Json;
using KnifeZ.Virgo.Core.Support.FileHandlers;
using KnifeZ.Virgo.Mvc;
using KnifeZ.Virgo.Mvc.Extensions;
using KnifeZ.Virgo.Mvc.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace WebDemo
{
    public class Startup
    {
        public Startup (IWebHostEnvironment env)
        {
            var configBuilder = new ConfigurationBuilder();
            ConfigRoot = configBuilder.AddKnifeJsonConfig(env).Build();
        }
        /// <summary>
        /// 可配置路径的Configuration
        /// </summary>
        public IConfigurationRoot ConfigRoot { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebDemo API", Version = "v1" });
                c.UseSwaggerSecurityOptions();
            });

            services.AddVirgoService(
                config: ConfigRoot, options: options =>
               {
                   options.DataPrivileges = DataPrivilegeSettings();
               });

            services.AddVirgoMultiLanguages();

            services.AddMvc(options =>
            {
                options.UseVirgoMvcOptions();
            })
            .AddJsonOptions(options =>
            {
                options.UseVirgoJsonOptions();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.UseVirgoApiOptions();
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddVirgoDataAnnotationsLocalization(typeof(Program));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptions<Configs>>().Value;

            if (configs == null)
            {
                throw new InvalidOperationException("Can not find Configs service, make sure you call AddVirgoService at ConfigService");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebDemo API V1");
                });
            }
            //自动跳转https
            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseVirgoService();
            app.UseVirgoMultiLanguages();

            app.UseAuthorization();
            app.UseAuthentication();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                   name: "areaRoute",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapFallbackToFile("index.html");
            });
        }

        /// <summary>
        /// 设置系统支持的数据权限
        /// </summary>
        /// <returns>data privileges list</returns>
        public List<IDataPrivilege> DataPrivilegeSettings ()
        {
            List<IDataPrivilege> pris = new List<IDataPrivilege>
            {
                //Add data privilege to specific type
                //指定哪些模型需要数据权限
                new DataPrivilegeInfo<FrameworkDomain>("域权限", m => m.Name)
            };
            return pris;
        }
    }
}
